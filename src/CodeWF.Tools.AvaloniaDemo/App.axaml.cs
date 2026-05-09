using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CodeWF.Log.Core;

using CodeWF.Tools.AvaloniaDemo.ViewModels;
using CodeWF.Tools.AvaloniaDemo.Views;

namespace CodeWF.Tools.AvaloniaDemo;

public partial class App : Application
{
    public static Assembly? ExecutingAssembly;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };

            // 程序退出前刷新日志缓冲区，确保测试过程完整落盘。
            desktop.Exit += async (_, _) => await Logger.FlushAsync();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
