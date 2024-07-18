using System.Diagnostics;
using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.Test;

public class ActionHelperTest
{
    [Fact]
    public void Test_CheckNormalAction_Success()
    {
        var check = ActionHelper.CheckOvertime(DoSomething, 400);
        Assert.True(check);
        return;

        void DoSomething()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(300));
        }
    }

    [Fact]
    public void Test_CheckOvertimeAction_Success()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var check = ActionHelper.CheckOvertime(DoSomething, 3000);
        Debug.WriteLine("Cancel running");
        cancellationTokenSource.Cancel();
        Assert.False(check);
        Debug.WriteLine("Check over");
        Thread.Sleep(TimeSpan.FromSeconds(5));
        Debug.WriteLine("End Test");
        return;

        void DoSomething()
        {
            var beginTime = DateTime.Now;
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                Debug.WriteLine($"{beginTime.GetDiffTime(DateTime.Now)}: Test overtime");
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }
        }
    }
}