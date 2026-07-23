using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Avalonia;
using CodeWF.Log.Core;
using CodeWF.Tools.AvaloniaDemo.Models;
using ReactiveUI.Avalonia;

namespace CodeWF.Tools.AvaloniaDemo;

internal static class Program
{
    // Native AOT 发布时保留 Demo 中需要展示和序列化的模型成员。
    [STAThread]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Project))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Member))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(List<Member>))]
    public static void Main(string[] args)
    {
        Logger.Initialize(new LoggerOptions
        {
            EnableConsole = false,
            File = new FileLogOptions
            {
                DirectoryPath = Path.Combine(Environment.CurrentDirectory, "Log")
            }
        });

        try
        {
            App.ExecutingAssembly = Assembly.GetExecutingAssembly();
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        finally
        {
            Logger.ShutdownAsync().GetAwaiter().GetResult();
        }
    }

    // Avalonia 桌面程序入口，保持启动阶段不访问 UI 线程相关对象。
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI(_ => { });
    }
}
