using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;
using Xunit;

namespace CodeWF.Tools.Test;

public class IpHelperTest
{
    [Fact]
    public void Test_GetLocalIpAsync_Success()
    {
        var ip = IpHelper.GetLocalIpAsync().Result;
        Assert.False(ip.IsNullOrEmpty());
    }

    [Fact]
    public void Test_GetAllIpv4Async_Success()
    {
        var ip = IpHelper.GetAllIpV4Async().Result;
        Assert.False(ip.IsNullOrEmpty());
    }

    [Fact]
    public void Test_GetAvailableTcpPort_Success()
    {
        var port = IpHelper.GetAvailableTcpPort();
        Assert.True(port > 0);
    }

    [Fact]
    public void Test_GetMulticastIpAndPort_Success()
    {
        IpHelper.GetMulticastIpAndPort(out var ip, out var port, needConnectCheck: true);
        Assert.False(ip.IsNullOrEmpty());
        Assert.False(port == default);
    }
}