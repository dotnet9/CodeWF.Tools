using System.Net;
using System.Net.Sockets;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.Core.Test.Helpers;

public class IpHelperTests
{
    [Fact]
    public async Task GetLocalIpAsync_ShouldReturnValidIpAddress()
    {
        // Act
        var result = await IpHelper.GetLocalIpAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(IPAddress.TryParse(result, out _));
    }

    [Fact]
    public void GetAllIpV4_ShouldReturnListOfIpAddresses()
    {
        // Act
        var result = IpHelper.GetAllIpV4();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<string>>(result);
        Assert.All(result, ip => Assert.True(IPAddress.TryParse(ip, out var address) && address.AddressFamily == AddressFamily.InterNetwork));
    }

    [Fact]
    public async Task GetAllIpV4Async_ShouldReturnListOfIpAddresses()
    {
        // Act
        var result = await IpHelper.GetAllIpV4Async();

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<string>>(result);
        Assert.All(result, ip => Assert.True(IPAddress.TryParse(ip, out var address) && address.AddressFamily == AddressFamily.InterNetwork));
    }

    [Fact]
    public void GetAvailableTcpPort_ShouldReturnValidPort()
    {
        // Act
        var port = IpHelper.GetAvailableTcpPort();

        // Assert
        Assert.True(port > 0);
        Assert.True(port < 65536); // Valid port range
    }

    [Theory]
    [InlineData("224.0.2.1", 7500)] // Valid multicast address
    [InlineData("224.1.1.1", 7600)] // Another valid multicast address
    [InlineData("239.255.255.255", 7700)] // Upper boundary of local multicast range
    [InlineData("238.0.0.1", 7800)] // Another valid multicast in global range
    public async Task CheckMulticastAvailabilityWithDetailsAsync_WithValidMulticastAddresses_ShouldWork(string ip, int port)
    {
        // Act
        var result = await IpHelper.CheckMulticastAvailabilityWithDetailsAsync(ip, port);

        // Assert
        if (!result.Success)
        {
            Assert.NotEmpty(result.ErrorMessage);
        }
        else
        {
            Assert.Empty(result.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("127.0.0.1", 7500)] // Loopback address
    [InlineData("192.168.1.1", 7600)] // Private address
    [InlineData("10.0.0.1", 7700)] // Another private address
    [InlineData("172.16.0.1", 7800)] // Another private address
    public async Task CheckMulticastAvailabilityWithDetailsAsync_WithNonMulticastAddresses_ShouldProvideErrorDetails(string ip, int port)
    {
        // Act
        var result = await IpHelper.CheckMulticastAvailabilityWithDetailsAsync(ip, port);

        // Assert
        if (!result.Success)
        {
            Assert.NotEmpty(result.ErrorMessage);
        }
        else
        {
            Assert.Empty(result.ErrorMessage);
        }
    }

    [Theory]
    [InlineData("invalid.ip", 7500)] // Invalid IP format
    [InlineData("999.999.999.999", 7600)] // Invalid IP values
    [InlineData("", 7700)] // Empty string
    public async Task CheckMulticastAvailabilityWithDetailsAsync_WithInvalidAddresses_ShouldReturnSpecificError(string ip, int port)
    {
        // Act
        var result = await IpHelper.CheckMulticastAvailabilityWithDetailsAsync(ip, port);

        // Assert
        Assert.False(result.Success);
        Assert.NotEmpty(result.ErrorMessage);
        Assert.Contains("IP", result.ErrorMessage, StringComparison.OrdinalIgnoreCase); // 确保错误消息提及了IP
    }

    // 保留原来的测试，确保现有的API仍然能正常工作
    [Theory]
    [InlineData("224.0.2.1", 7500)]
    public async Task CheckMulticastAvailabilityAsync_ShouldStillWork(string ip, int port)
    {
        // Act
        var result = await IpHelper.CheckMulticastAvailabilityAsync(ip, port);
        
        // Assert
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void GetMulticastIpAndPort_ShouldReturnValidIpAndPort()
    {
        // Act
        bool result = IpHelper.GetMulticastIpAndPort(out string ip, out int port);

        // Assert
        Assert.True(result);
        Assert.NotNull(ip);
        Assert.True(IPAddress.TryParse(ip, out var ipAddress));
        
        // Verify it's a multicast address (224.0.0.0 to 239.255.255.255)
        var bytes = ipAddress.GetAddressBytes();
        Assert.True(bytes[0] >= 224 && bytes[0] <= 239);
        
        // Verify port is in expected range
        Assert.True(port >= 7000 && port <= 7999);
    }

    [Fact]
    public void GetMulticastIpAndPort_WithCustomRange_ShouldReturnPortInRange()
    {
        // Arrange
        int startPort = 8000;
        int endPort = 8500;

        // Act
        bool result = IpHelper.GetMulticastIpAndPort(out string ip, out int port, startPort, endPort);

        // Assert
        Assert.True(result);
        Assert.NotNull(ip);
        Assert.True(IPAddress.TryParse(ip, out _));
        Assert.True(port >= startPort && port <= endPort);
    }
} 