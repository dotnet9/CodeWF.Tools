using CodeWF.Tools.Extensions;
using CodeWF.Tools.Test.Models;

namespace CodeWF.Tools.Test;

public class ObjectExtensionsTest
{
    [Fact]
    public void Test_GetClassDescription_Success()
    {
        var desc = ObjectExtension.GetDescription<RequestProducts>();

        Assert.Equal("请求产品列表", desc);
    }
}