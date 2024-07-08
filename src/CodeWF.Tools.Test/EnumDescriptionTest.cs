using CodeWF.Tools.Extensions;
using CodeWF.Tools.Test.Models;

namespace CodeWF.Tools.Test;

[TestClass]
public class EnumDescriptionTest
{
    [TestMethod]
    public void Test_GetEnumDescription_Success()
    {
        var usage = PowerUsage.High;
        var alarmStatus = AlarmStatus.OverLimit | AlarmStatus.UserChanged;

        var usageDescription = usage.GetDescription();
        var alarmStatusDescription = alarmStatus.GetDescription();

        Assert.AreEqual("高", usageDescription);
        Assert.AreEqual("超限,切换用户", alarmStatusDescription);

        alarmStatus = AlarmStatus.Normal;
        alarmStatusDescription = alarmStatus.GetDescription();
        Assert.AreEqual("正常", alarmStatusDescription);
    }
}