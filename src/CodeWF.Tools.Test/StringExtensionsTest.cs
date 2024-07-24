using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;
using Xunit;

namespace CodeWF.Tools.Test;

public class StringExtensionsTest
{
    [Fact]
    public void Test_OpenBrowserForVisitSite_Success()
    {
        "https://codewf.com".OpenBrowserForVisitSite();
    }
}