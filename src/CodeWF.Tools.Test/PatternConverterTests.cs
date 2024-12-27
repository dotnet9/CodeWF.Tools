namespace CodeWF.Tools.Test;

using Xunit;

/// <summary>
/// 针对 <see cref="CodeWF.Tools.Extensions.RegexExtensions"/> 类的测试类，
/// 全面测试 <see cref="RegexExtensions.ConvertPattern"/> 和 <see cref="RegexExtensions.IsMatch"/> 方法。
/// </summary>
public class PatternConverterTests
{
    #region 测试特殊字符替换

    /// <summary>
    /// 测试 '*' 字符替换为正则表达式片段 '.+' 是否正确。
    /// 同时验证 <see cref="RegexExtensions.IsMatch"/> 方法在该替换后的匹配结果。
    /// </summary>
    [Fact]
    public void TestAsteriskReplacement()
    {
        var pattern = "*abc";
        // 预期能匹配包含 "abc" 的字符串
        Assert.True("xyzabc".IsMatch(pattern));
        // 预期不能匹配不包含 "abc" 的字符串
        Assert.False("xyz".IsMatch(pattern));
        Assert.True("abcd".IsMatch("*cd"));
        Assert.True("abcd".IsMatch("ab*"));
        Assert.True("abcd".IsMatch("*bc*"));
    }

    /// <summary>
    /// 测试 '%' 字符替换为正则表达式片段 '.+' 是否正确，
    /// 以及对应的 <see cref="RegexExtensions.IsMatch"/> 匹配效果。
    /// </summary>
    [Fact]
    public void TestPercentReplacement()
    {
        string pattern = "%def";
        Assert.True("abcdef".IsMatch(pattern));
        Assert.False("abc".IsMatch(pattern));
    }

    /// <summary>
    /// 测试 '#' 字符替换为正则表达式片段 '\\d' 是否准确，
    /// 并检查 <see cref="RegexExtensions.IsMatch"/> 的匹配正确性。
    /// </summary>
    [Fact]
    public void TestHashReplacement()
    {
        string pattern = "#3";
        Assert.True("123abc".IsMatch(pattern));
        Assert.False("abc".IsMatch(pattern));
    }

    /// <summary>
    /// 测试 '@' 字符替换为正则表达式片段 '[a-zA-Z]' 是否无误，
    /// 核实 <see cref="RegexExtensions.IsMatch"/> 的匹配结果。
    /// </summary>
    [Fact]
    public void TestAtReplacement()
    {
        string pattern = "@b";
        Assert.True("abc".IsMatch(pattern));
        Assert.False("123".IsMatch(pattern));
    }

    /// <summary>
    /// 测试 '?' 字符替换为正则表达式片段 '\\w' 是否正确，
    /// 验证 <see cref="RegexExtensions.IsMatch"/> 的匹配情况。
    /// </summary>
    [Fact]
    public void TestQuestionMarkReplacement()
    {
        string pattern = "?b";
        Assert.True("ab".IsMatch(pattern));
        Assert.True("1b".IsMatch(pattern));
    }

    #endregion

    #region 测试组合模式

    /// <summary>
    /// 测试多个特殊字符组合后的模式转换及匹配效果，
    /// 检验 <see cref="RegexExtensions.ConvertPattern"/> 和 <see cref="RegexExtensions.IsMatch"/> 协同工作情况。
    /// </summary>
    [Fact]
    public void TestCombinedPattern()
    {
        string pattern = "a*#c@d";
        Assert.True("aabb123cxd".IsMatch(pattern));
        Assert.True("aab12cxd".IsMatch(pattern));
    }

    #endregion

    #region 测试转义字符

    /// <summary>
    /// 测试转义字符 '\\' 是否能阻止特殊字符被替换，
    /// 确保 <see cref="RegexExtensions.ConvertPattern"/> 对转义字符处理正确，
    /// 以及 <see cref="RegexExtensions.IsMatch"/> 给出合适匹配结果。
    /// </summary>
    [Fact]
    public void TestEscapeCharacter()
    {
        string pattern = "\\*abc";
        Assert.False("*abc".IsMatch(pattern));
        Assert.True("\\*abc".IsMatch(pattern));
    }

    #endregion

    #region 测试原始正则表达式语义

    /// <summary>
    /// 测试输入普通正则表达式时，<see cref="RegexExtensions.ConvertPattern"/> 不改变其语义，
    /// 且 <see cref="RegexExtensions.IsMatch"/> 能正常匹配。
    /// </summary>
    [Fact]
    public void TestOriginalRegexSemantics()
    {
        string pattern = "a+b*";
        Assert.True("aabb".IsMatch(pattern));
        Assert.False("ab".IsMatch(pattern));

        pattern = "a{2,3}";
        Assert.True("aaa".IsMatch(pattern));
        Assert.False("a".IsMatch(pattern));
    }

    #endregion
}