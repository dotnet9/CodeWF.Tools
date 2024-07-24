using Xunit;

namespace CodeWF.Tools.Test;

public class NumberExtensionsTest
{
    [Fact]
    public void Test_NumberFormat_Success()
    {
        const double value = 1234567.8999;

        var factDefault = $"{value}";
        var factFiveThree = value.ToString("00.000e+00");
        var factSevenTwo = value.ToString("00000.00e+00");

        Assert.Equal("1234567.8999", factDefault);
        Assert.Equal("12.346e+05", factFiveThree);
        Assert.Equal("12345.68e+02", factSevenTwo);
    }
}