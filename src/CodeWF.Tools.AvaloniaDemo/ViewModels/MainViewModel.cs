using CodeWF.Tools.Extensions;

namespace CodeWF.Tools.AvaloniaDemo.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public string? AssemblyInfo { get; set; }

    public MainViewModel()
    {
        AssemblyInfo = $"{nameof(ThisAssembly.Title)}: {ThisAssembly.Title}\r\n" +
                       $"{nameof(ThisAssembly.Description)}: {ThisAssembly.Description}\r\n" +
                       $"{nameof(ThisAssembly.Company)}: {ThisAssembly.Company}\r\n" +
                       $"{nameof(ThisAssembly.Product)}: {ThisAssembly.Product}\r\n" +
                       $"{nameof(ThisAssembly.Version)}: {ThisAssembly.Version}\r\n" +
                       $"{nameof(ThisAssembly.InformationalVersion)}: {ThisAssembly.InformationalVersion}\r\n" +
                       $"{nameof(ThisAssembly.FileVersion)}: {ThisAssembly.FileVersion}\r\n" +
                       $"{nameof(ThisAssembly.Copyright)}: {ThisAssembly.Copyright}\r\n" +
                       $"{nameof(ThisAssembly.CompileTime)}: {ThisAssembly.CompileTime:yyyy-MM-dd HH:mm:ss}\r\n";
    }
}
