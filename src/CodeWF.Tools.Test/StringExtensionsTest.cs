using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.Test;

[TestClass]
public class StringExtensionsTest
{
    [TestMethod]
    public void Test_OpenBrowserForVisitSite_Success()
    {
        "https://codewf.com".OpenBrowserForVisitSite();
    }
}