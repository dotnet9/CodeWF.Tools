using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeWF.Tools.Extensions;

public static class RegexExtensions
{
    public static bool IsMatch(this string data, string pattern)
    {
        var patterns = SplitPatterns(pattern).ToArray();
        if (patterns.Length > 1)
        {
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
                converted.Append(Regex.Escape(currentChar.ToString()));
                if ((i + 1) < pattern.Length)
                {
                    converted.Append(Regex.Escape(pattern[i + 1].ToString()));
                    i++;
                }

                continue;
            }

            switch (currentChar)
            {
                case '*':
                case '%':
                    converted.Append(AllowsEmptyWildcard(pattern, i) ? ".*" : ".+");
                    break;
                case '#':
                    converted.Append(@"\d");
                    break;
                case '@':
                    converted.Append("[a-zA-Z]");
                    break;
                case '?':
                    converted.Append(@"\w");
                    break;
                default:
                    converted.Append(currentChar);
                    break;
            }
        }

        var startsWithWildcard = IsUnescapedWildcardAt(pattern, 0);
        var endsWithWildcard = IsUnescapedWildcardAt(pattern, pattern.Length - 1);

        if (startsWithWildcard && !endsWithWildcard)
        {
            converted.Append('$');
        }
        else if (endsWithWildcard && !startsWithWildcard)
        {
            converted.Insert(0, '^');
        }

        return converted.ToString();
    }

    private static IEnumerable<string> SplitPatterns(string pattern)
    {
        var current = new StringBuilder();
        var braceDepth = 0;
        var bracketDepth = 0;
        var escaped = false;

        foreach (var currentChar in pattern)
        {
            if (escaped)
            {
                current.Append(currentChar);
                escaped = false;
                continue;
            }

            if (currentChar == '\\')
            {
                current.Append(currentChar);
                escaped = true;
                continue;
            }

            switch (currentChar)
            {
                case '{':
                    braceDepth++;
                    break;
                case '}' when braceDepth > 0:
                    braceDepth--;
                    break;
                case '[':
                    bracketDepth++;
                    break;
                case ']' when bracketDepth > 0:
                    bracketDepth--;
                    break;
                case ',':
                case '，':
                    if (braceDepth == 0 && bracketDepth == 0)
                    {
                        if (current.Length > 0)
                        {
                            yield return current.ToString();
                            current.Clear();
                        }

                        continue;
                    }

                    break;
            }

            current.Append(currentChar);
        }

        if (current.Length > 0)
        {
            yield return current.ToString();
        }
    }

    private static bool AllowsEmptyWildcard(string pattern, int index)
    {
        return (index == 0 || index == pattern.Length - 1) && !ContainsRegexSyntax(pattern);
    }

    private static bool ContainsRegexSyntax(string pattern)
    {
        foreach (var currentChar in pattern)
        {
            switch (currentChar)
            {
                case '+':
                case '{':
                case '}':
                case '[':
                case ']':
                case '(':
                case ')':
                case '|':
                case '^':
                case '$':
                    return true;
            }
        }

        return false;
    }

    private static bool IsUnescapedWildcardAt(string pattern, int index)
    {
        if (index < 0 || index >= pattern.Length)
        {
            return false;
        }

        if (pattern[index] is not ('*' or '%'))
        {
            return false;
        }

        var slashCount = 0;
        for (var i = index - 1; i >= 0 && pattern[i] == '\\'; i--)
        {
            slashCount++;
        }

        return slashCount % 2 == 0;
    }
}
