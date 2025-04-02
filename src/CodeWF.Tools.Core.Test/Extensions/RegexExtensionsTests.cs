using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Core.Test.Extensions;
public class RegexExtensionsTests
{

    [Fact]
    public void IsMatch_ShouldFilterSuccess()
    {
        var data = new List<string>{"ab", "ab1", "bc", "1bc", "12abc", "45abc", "2abc8","3ab25abc235", "abc", "abc32"};

        // 测试包含abc的匹配
        var result = data.Where(item => item.IsMatch("abc")).ToList();
        Assert.Contains("12abc", result);
        Assert.Contains("45abc", result);
        Assert.Contains("2abc8", result);
        Assert.Contains("3ab25abc235", result);

        // 测试以abc结尾的匹配
        result = data.Where(item => item.IsMatch("*abc")).ToList();
        Assert.Contains("12abc", result);
        Assert.Contains("45abc", result);
        Assert.Contains("abc", result);
        Assert.DoesNotContain("2abc8", result);

        // 测试以abc开头的匹配
        result = data.Where(item => item.IsMatch("abc*")).ToList();
        Assert.Contains("abc", result);
        Assert.Contains("abc32", result);

        // 测试同时包含ab和bc的匹配
        result = data.Where(item => item.IsMatch("ab，bc")).ToList();
        Assert.Contains("ab", result);   
        Assert.Contains("ab1", result);  
        Assert.Contains("bc", result);   
        Assert.Contains("1bc", result);
        Assert.Contains("12abc", result);
        Assert.Contains("45abc", result);
        Assert.Contains("2abc8", result);
        Assert.Contains("3ab25abc235", result);
        Assert.Contains("abc", result);
        Assert.Contains("abc32", result);
    }
}
