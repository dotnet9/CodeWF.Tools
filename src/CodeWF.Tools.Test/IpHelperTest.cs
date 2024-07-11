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

    [TestMethod]
    public void Test_GetAllIpv4Async_Success()
    {
        var ip = IpHelper.GetAllIpV4Async().Result;
        Assert.IsFalse(ip.IsNullOrEmpty());
    }

    [TestMethod]
    public void Test_GetAvailableTcpPort_Success()
    {
        var port = IpHelper.GetAvailableTcpPort();
        Assert.IsTrue(port > 0);
    }

    [TestMethod]
    public void Test_GetMulticastIpAndPort_Success()
    {
        IpHelper.GetMulticastIpAndPort(out var ip, out var port, needConnectCheck: true);
        Assert.IsFalse(ip.IsNullOrEmpty());
        Assert.IsFalse(port == default);
    }
}