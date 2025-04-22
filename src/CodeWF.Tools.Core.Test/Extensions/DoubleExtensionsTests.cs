using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Core.Test.Extensions
{
    public class DoubleExtensionsTests
    {
        [Theory]
        [InlineData(500, 2, "500.00 B")]
        [InlineData(1024, 2, "1.00 KB")]
        [InlineData(1048576, 2, "1.00 MB")]
        [InlineData(1073741824, 2, "1.00 GB")]
        [InlineData(1099511627776.0, 2, "1.00 TB")]  // 使用 .0 确保是 double 类型
        [InlineData(1125899906842624.0, 2, "1.00 PB")] // 添加 PB 测试
        [InlineData(-1024, 2, "-1.00 KB")]
        [InlineData(1024, 1, "1.0 KB")]
        [InlineData(1024, 3, "1.000 KB")]
        public void FormatBytes_ShouldFormatSuccess(double bytes, int decimalPlaces, string expected)
        {
            var result = bytes.FormatBytes(decimalPlaces);
            Assert.Equal(expected, result);
        }
    }
}
