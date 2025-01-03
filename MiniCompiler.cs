using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

class MiniCompiler
{
    static void Main(string[] args)
    {
        Console.Write("Enter usernames (separated by commas): ");
        string input = Console.ReadLine();
        var usernames = input.Split(',').Select(u => u.Trim()).ToList();

        Dictionary<string, string> passwordStrengths = new();
        List<string> validUsernames = new();
        List<string> invalidUsernames = new();

        foreach (var username in usernames)
        {
            if (ValidateUsername(username, out string validationResult))
            {
                Console.WriteLine($"{username} - Valid");
                validUsernames.Add(username);
                GeneratePassword(username, out string password, out string strength);
                passwordStrengths[username] = $"{password} (Strength: {strength})";
            }
            else
            {
                Console.WriteLine($"{username} - Invalid ({validationResult})");
                invalidUsernames.Add(username);
            }
        }

        WriteToFile(usernames.Count, validUsernames, invalidUsernames, passwordStrengths);

        Console.WriteLine("\nInvalid Usernames: " + string.Join(", ", invalidUsernames));
        Console.Write("Do you want to retry invalid usernames? (y/n): ");
        if (Console.ReadLine().Trim().ToLower() == "y")
        {
            Console.Write("Enter invalid usernames: ");
            input = Console.ReadLine();
            usernames = input.Split(',').Select(u => u.Trim()).ToList();
            Main(new string[] { string.Join(",", usernames) });
        }
    }

    static bool ValidateUsername(string username, out string validationResult)
    {
        validationResult = string.Empty;

        if (!Regex.IsMatch(username, @"^[a-zA-Z]"))
        {
            validationResult = "Username must start with a letter";
            return false;
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]{5,15}$"))
        {
            validationResult = "Username length must be between 5 and 15";
            return false;
        }

        if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
        {
            validationResult = "Username can only contain letters, digits, and underscores";
            return false;
        }

        return true;
    }

    static void GeneratePassword(string username, out string password, out string strength)
    {
        Random random = new();
        string upper = RandomChars("ABCDEFGHIJKLMNOPQRSTUVWXYZ", 2, random);
        string lower = RandomChars("abcdefghijklmnopqrstuvwxyz", 2, random);
        string digits = RandomChars("0123456789", 2, random);
        string special = RandomChars("!@#$%^&*", 2, random);
        string any = RandomChars("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*", 4, random);

        password = new string((upper + lower + digits + special + any).OrderBy(_ => random.Next()).ToArray());

        int types = (upper.Any() ? 1 : 0) + (lower.Any() ? 1 : 0) + (digits.Any() ? 1 : 0) + (special.Any() ? 1 : 0);
        strength = password.Length >= 12 && types >= 4 ? "Strong" : "Medium";
    }

    static string RandomChars(string chars, int count, Random random)
    {
        return new string(Enumerable.Repeat(chars, count).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    static void WriteToFile(int total, List<string> valid, List<string> invalid, Dictionary<string, string> passwords)
    {
        StringBuilder fileContent = new();
        fileContent.AppendLine("Validation Results:");
        foreach (var username in valid)
        {
            fileContent.AppendLine($"{username} - Valid");
            fileContent.AppendLine($"Generated Password: {passwords[username]}");
        }
        foreach (var username in invalid)
        {
            fileContent.AppendLine($"{username} - Invalid");
        }
        fileContent.AppendLine($"\nSummary:");
        fileContent.AppendLine($"- Total Usernames: {total}");
        fileContent.AppendLine($"- Valid Usernames: {valid.Count}");
        fileContent.AppendLine($"- Invalid Usernames: {invalid.Count}");

        File.WriteAllText("UserDetails.txt", fileContent.ToString());
        Console.WriteLine("\nResults saved to UserDetails.txt");
    }
}
