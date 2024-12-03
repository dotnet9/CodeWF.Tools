using System.Reflection;
using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.AvaloniaDemo.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public string? AssemblyInfo { get; set; }

    public MainViewModel()
    {
        AssemblyInfo = $"{nameof(AssemblyExtensions.Title)}: {App.ExecutingAssembly.Title()}\r\n" +
                       $"{nameof(AssemblyExtensions.Description)}: {App.ExecutingAssembly.Description()}\r\n" +
                       $"{nameof(AssemblyExtensions.Company)}: {App.ExecutingAssembly.Company()}\r\n" +
                       $"{nameof(AssemblyExtensions.Product)}: {App.ExecutingAssembly.Product()}\r\n" +
                       $"{nameof(AssemblyExtensions.Version)}: {App.ExecutingAssembly.Version()}\r\n" +
                       $"{nameof(AssemblyExtensions.InformationalVersion)}: {App.ExecutingAssembly.InformationalVersion()}\r\n" +
                       $"{nameof(AssemblyExtensions.FileVersion)}: {App.ExecutingAssembly.FileVersion()}\r\n" +
                       $"{nameof(AssemblyExtensions.Copyright)}: {App.ExecutingAssembly.Copyright()}\r\n" +
                       $"{nameof(AssemblyExtensions.CompileTime)}: {App.ExecutingAssembly.CompileTime():yyyy-MM-dd HH:mm:ss}\r\n";
    }
}