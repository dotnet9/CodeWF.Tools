using CodeWF.Tools.FileExtensions;
using System.Text;

namespace CodeWF.Tools.Core.Test
{
    public class FileHelperTest
    {
        // 静态构造函数，在类的所有实例创建之前执行，用于注册编码提供程序
        static FileHelperTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Fact]
        public void Test_CheckFileCode_Success()
        {
            // 存储文件名和预期编码的映射关系
            var fileNameToEncodingMap = new Dictionary<string, Encoding>()
            {
                { "ansi.txt", Encoding.GetEncoding("GB2312") },
                { "gb2312.txt", Encoding.GetEncoding("GB2312") },
                { "utf8.txt", Encoding.UTF8 },
                { "utf8BOM.txt", Encoding.UTF8 }
            };

            // 遍历文件名和预期编码的映射关系
            foreach (var fileNameEncodingPair in fileNameToEncodingMap)
            {
                // 获取文件名
                var fileName = fileNameEncodingPair.Key;
                // 获取预期编码
                var expectedEncoding = fileNameEncodingPair.Value;

                // 使用更现代的方式获取测试数据路径
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "TestDatas", fileName);

                // 确保文件存在
                Assert.True(File.Exists(filePath), $"文件 {filePath} 不存在。");

                try
                {
                    // 获取文件编码类型
                    var detectedEncoding = FileHelper.GetFileEncodeType(filePath);

                    // 断言获取的编码与预期编码一致，并输出详细信息
                    Assert.Equal(expectedEncoding, detectedEncoding);
                }
                catch (Exception ex)
                {
                    // 捕获异常并输出详细信息
                    Assert.True(false, $"处理文件 {filePath} 时发生异常: {ex.Message}");
                }
            }
        }
    }
}