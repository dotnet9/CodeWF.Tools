#nullable enable
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CodeWF.Tools.Helpers;

public static class IpHelper
{
    public static async Task<string?> GetLocalIpAsync()
    {
        var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync("https://ipv4.gdt.qq.com/get_client_ip");
        var str = await response.Content.ReadAsStringAsync();
        return IPAddress.TryParse(str, out var ip) ? ip.ToString() : string.Empty;
    }
}