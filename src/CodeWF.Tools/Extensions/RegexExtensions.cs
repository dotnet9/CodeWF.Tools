using System.Text;
using System.Text.RegularExpressions;

namespace CodeWF.Tools.Extensions;

/// <summary>
/// 提供正则表达式相关的字符串扩展方法
/// </summary>
public static class RegexExtensions
{
    /// <summary>
    /// 验证给定数据是否匹配经过转换后的正则表达式模式
    /// </summary>
    /// <param name="data">要验证的数据字符串</param>
    /// <param name="pattern">原始的模式字符串</param>
    /// <returns>如果数据匹配转换后的正则表达式则返回true，否则返回false</returns>
    public static bool IsMatch(this string data, string pattern)
    {
        var convertedPattern = pattern.ConvertPattern();
        var regex = new Regex(convertedPattern, RegexOptions.IgnoreCase);
        return regex.IsMatch(data);
    }

    /// <summary>
    /// 将特定字符按照自定义规则转换为正则表达式片段
    /// 例如，把`*`转换为`.+`，`%`转换为`.+`等，同时处理转义字符
    /// </summary>
    /// <param name="pattern">需要转换的原始字符串</param>
    /// <returns>转换后的字符串，可用于构建正则表达式</returns>
    public static string ConvertPattern(this string pattern)
    {
        var converted = new StringBuilder();
        for (var i = 0; i < pattern.Length; i++)
        {
            var currentChar = pattern[i];
            if (currentChar == '\\')
            {
                // 如果遇到转义字符，且后面还有字符，就直接添加后面的字符，跳过替换逻辑
                if ((i + 1) < pattern.Length)
                {
                    converted.Append(pattern[i + 1]);
                    i++;
                }
                else
                {
                    // 如果\是最后一个字符，直接添加它，避免格式错误
                    converted.Append(currentChar);
                }

                continue;
            }

            switch (currentChar)
            {
                case '*':
                case '%':
                    if (converted.Length > 0)
                    {
                        converted.Append(char.IsLetterOrDigit(converted[^1]) ? ".+" : "(.+)");
                    }
                    else
                    {
                        converted.Append("^(.+)");
                    }
                    break;
                case '#':
                    if (converted.Length > 0)
                    {
                        converted.Append(char.IsLetterOrDigit(converted[^1]) ? "\\d" : "(\\d)");
                    }
                    else
                    {
                        converted.Append("^\\d");
                    }
                    break;
                case '@':
                    if (converted.Length > 0)
                    {
                        converted.Append(char.IsLetterOrDigit(converted[^1]) ? "[a-zA-Z]" : "([a-zA-Z])");
                    }
                    else
                    {
                        converted.Append("^[a-zA-Z]");
                    }
                    break;
                case '?':
                    if (converted.Length > 0)
                    {
                        converted.Append(char.IsLetterOrDigit(converted[^1]) ? "[a-zA-Z0-9_]" : "([a-zA-Z0-9_])");
                    }
                    else
                    {
                        converted.Append("^[a-zA-Z0-9_]");
                    }
                    break;
                default:
                    converted.Append(currentChar);
                    break;
            }
        }

        return converted.ToString();
    }
}