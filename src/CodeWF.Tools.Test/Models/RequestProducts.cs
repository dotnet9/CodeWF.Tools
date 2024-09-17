using System.ComponentModel;

namespace CodeWF.Tools.Test.Models;

[Description("请求产品列表")]
public class RequestProducts
{
    public string? Name { get; set; }
    public int Year { get; set; }
}