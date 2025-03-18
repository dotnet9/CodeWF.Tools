using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.Helpers;

public static class IpHelper
{
    /// <summary>
    ///     获取本地IP地址详细信息
    /// </summary>
    /// <returns></returns>
    public static async Task<string?> GetLocalIpAsync()
    {
        var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync("https://ipv4.gdt.qq.com/get_client_ip");
        var str = await response.Content.ReadAsStringAsync();
        return IPAddress.TryParse(str, out var ip) ? ip.ToString() : string.Empty;
    }

    /// <summary>
    ///     获取本地所有IP V4地址
    /// </summary>
    /// <returns></returns>
    public static List<string> GetAllIpV4()
    {
        var ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
        return ipAddresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork)
            .Select(address => address.ToString()).ToList();
    }

    /// <summary>
    ///     获取本地所有IP V4地址
    /// </summary>
    /// <returns></returns>
    public static async Task<List<string>> GetAllIpV4Async()
    {
        var ipAddresses = await Dns.GetHostAddressesAsync(Dns.GetHostName());
        return ipAddresses.Where(address => address.AddressFamily == AddressFamily.InterNetwork)
            .Select(address => address.ToString()).ToList();
    }

    /// <summary>
    ///     获取本机可使用的一个TCP端口
    /// </summary>
    /// <returns></returns>
    public static int GetAvailableTcpPort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    /// <summary>
    ///     验证UDP地址是否可用（支持组播和普通地址）
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口</param>
    /// <param name="errorMsg">失败时的错误信息</param>
    /// <returns>成功返回true，失败返回false</returns>
    public static bool CheckMulticastAvailability(string ip, int port, out string errorMsg)
    {
        errorMsg = string.Empty;
        
        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            errorMsg = $"IP地址 '{ip}' 格式无效";
            return false;
        }

        UdpClient? udpClient = null;
        try
        {
            udpClient = new UdpClient();
            
            // 只有对多播地址才调用JoinMulticastGroup
            byte firstByte = ipAddress.GetAddressBytes()[0];
            bool isMulticast = firstByte >= 224 && firstByte <= 239;
            
            if (isMulticast)
            {
                udpClient.JoinMulticastGroup(ipAddress);
            }
            
            var endPoint = new IPEndPoint(ipAddress, port);
            var buffer = Encoding.Default.GetBytes("udp test");
            udpClient.Send(buffer, buffer.Length, endPoint);
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
            return false;
        }
        finally
        {
            var isAvailable = udpClient?.Client?.Available ?? 0;
            if (isAvailable > 0) udpClient?.Client?.Shutdown(SocketShutdown.Both);

            udpClient?.Client?.Close();
            udpClient?.Client?.Dispose();
            udpClient?.Close();
            udpClient = null;
        }

        return true;
    }

    /// <summary>
    ///     验证UDP组播地址是否可用，并返回结果和错误信息
    /// </summary>
    /// <param name="ip">组播IP</param>
    /// <param name="port">组播端口</param>
    /// <returns>返回一个元组，包含成功标志和错误信息</returns>
    public static async Task<(bool Success, string ErrorMessage)> CheckMulticastAvailabilityWithDetailsAsync(string ip, int port)
    {
        if (!IPAddress.TryParse(ip, out var ipAddress))
        {
            return (false, $"IP地址 '{ip}' 格式无效");
        }

        UdpClient? udpClient = null;
        try
        {
            udpClient = new UdpClient();
            
            // 只有对多播地址才调用JoinMulticastGroup
            byte firstByte = ipAddress.GetAddressBytes()[0];
            bool isMulticast = firstByte >= 224 && firstByte <= 239;
            
            if (isMulticast)
            {
                udpClient.JoinMulticastGroup(ipAddress);
            }
            
            var endPoint = new IPEndPoint(ipAddress, port);
            var buffer = Encoding.Default.GetBytes("udp test");
            await udpClient.SendAsync(buffer, buffer.Length, endPoint);
            
            return (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
        finally
        {
            var isAvailable = udpClient?.Client?.Available ?? 0;
            if (isAvailable > 0) udpClient?.Client?.Shutdown(SocketShutdown.Both);

            udpClient?.Client?.Close();
            udpClient?.Client?.Dispose();
            udpClient?.Close();
            udpClient = null;
        }
    }

    /// <summary>
    ///     验证UDP组播地址是否可用
    /// </summary>
    /// <param name="ip">组播IP</param>
    /// <param name="port">组播端口</param>
    /// <returns>成功返回true，失败返回false</returns>
    public static async Task<bool> CheckMulticastAvailabilityAsync(string ip, int port)
    {
        var result = await CheckMulticastAvailabilityWithDetailsAsync(ip, port);
        return result.Success;
    }

    /// <summary>
    ///     获取可用的UDP组播地址
    /// </summary>
    /// <param name="ip">返回组播可用的IP</param>
    /// <param name="port">返回组播可用的端口</param>
    /// <param name="startPort">限制组播可用的端口起始值</param>
    /// <param name="endPort">限制组播可用的端口终止值</param>
    /// <param name="needConnectCheck">得到地址后，是否需要连接验证，一般不需要</param>
    /// <returns></returns>
    public static bool GetMulticastIpAndPort(out string ip, out int port, int startPort = 7000, int endPort = 7999,
        bool needConnectCheck = false)
    {
        while (true)
        {
            // 多播、组播
            // 239.0.0.0 - 239.255.255.255 本地
            // 224.0.2.0 - 238.255.255.255 用户可用，全网范围
            // 224.0.0.1 - 224.0.0.255 预留地址，最好不用
            ip =
                $"{RandomExtension.GetInt(224, 238)}.{RandomExtension.GetInt(0, 255)}.{RandomExtension.GetInt(2, 255)}.{RandomExtension.GetInt(0, 255)}";
            var tempPort = startPort;

            var udpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
            var tcpListeners = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();
            while (udpListeners.Any(listener => listener.Port == tempPort) ||
                   tcpListeners.Any(listener => listener.Port == tempPort))
                tempPort++;

            port = tempPort;
            if (!needConnectCheck || CheckMulticastAvailabilityAsync(ip, tempPort).Result) return true;
            needConnectCheck = false;
            continue;

            break;
        }
    }
}