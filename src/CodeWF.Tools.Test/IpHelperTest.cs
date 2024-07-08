using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.Test;

[TestClass]
public class IpHelperTest
{
    [TestMethod]
    public void Test_GetLocalIpAsync_Success()
    {
        var ip = IpHelper.GetLocalIpAsync().Result;
        Assert.IsFalse(ip.IsNullOrEmpty());
    }
}