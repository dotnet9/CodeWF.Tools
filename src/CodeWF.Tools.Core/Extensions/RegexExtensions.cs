using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeWF.Tools.Extensions;

public static class RegexExtensions
{
    public static bool IsMatch(this string data, string pattern)
    {
        if (pattern.Contains(',') || pattern.Contains('，'))
        {
            var patterns = pattern.Split([',', '，'], StringSplitOptions.RemoveEmptyEntries);
            return patterns.Any(p => IsMatch(data, p.Trim()));
        }

        var convertedPattern = pattern.ConvertPattern();
        var regex = new Regex(convertedPattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(data);
    }

    public static string ConvertPattern(this string pattern)
    {
        var converted = new StringBuilder();
        for (var i = 0; i < pattern.Length; i++)
        {
            var currentChar = pattern[i];
            if (currentChar == '\\')
            {
                if ((i + 1) < pattern.Length)
                {
                    converted.Append(pattern[i + 1]);
                    i++;
                }
                else
                {
                    converted.Append(currentChar);
                }

                continue;
            }

            switch (currentChar)
            {
                case '*':
                case '%':
                    if (i == 0 && pattern.Length > 1)
                    {
                        var endPattern = pattern.Substring(1);
                        converted.Append($".*{Regex.Escape(endPattern)}$");
                        i = pattern.Length;
                    }
                    else if (i == pattern.Length - 1 && pattern.Length > 1)
                    {
                        var startPattern = pattern.Substring(0, pattern.Length - 1);
                        converted.Clear();
                        converted.Append($"^{Regex.Escape(startPattern)}.*");
                        i = pattern.Length;
                    }
                    else
                    {
                        converted.Append(".*");
                    }
                    break;
                default:
                    if (i == 0)
                    {
                        converted.Append(".*");
                    }
                    converted.Append(Regex.Escape(currentChar.ToString()));
                    if (i == pattern.Length - 1)
                    {
                        converted.Append(".*");
                    }
                    break;
            }
        }

        return converted.ToString();
    }
}