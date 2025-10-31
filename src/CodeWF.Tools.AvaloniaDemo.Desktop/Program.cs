using Avalonia;
using CodeWF.Tools.AvaloniaDemo.Models;
using ReactiveUI.Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeWF.Tools.AvaloniaDemo.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Project))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Member))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(List<Member>))]
    public static void Main(string[] args)
    {
        App.ExecutingAssembly = Assembly.GetExecutingAssembly();
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}