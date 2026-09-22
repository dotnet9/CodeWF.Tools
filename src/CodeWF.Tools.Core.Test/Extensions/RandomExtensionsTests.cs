using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Core.Test.Extensions;

public class RandomExtensionsTests
{
    [Fact]
    public void GetInt_ReturnsValuesWithinTheRequestedRange()
    {
        for (var i = 0; i < 1000; i++)
        {
            var value = RandomExtension.GetInt(10, 20);

            Assert.InRange(value, 10, 19);
        }
    }

    [Fact]
    public void GetInt_WithInvalidRange_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => RandomExtension.GetInt(20, 10));
    }
}
