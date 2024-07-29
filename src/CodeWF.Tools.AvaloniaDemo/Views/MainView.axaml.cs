using Avalonia.Controls;
using Avalonia.Interactivity;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.AvaloniaDemo.Views;

public partial class MainView : UserControl
{
    private readonly TextBlock? _txtMainThemeConfig;
    private readonly TextBlock? _txtChildThemeConfig;
    private const string ThemeKey = "Theme";
    private readonly AppConfigHelper _entryAssemblyConfigHelper;
    private readonly AppConfigHelper _CallingAssemblyConfigHelper;

    public MainView()
    {
        InitializeComponent();
        var txtEntryAssemblyConfig = this.FindControl<TextBlock>("TxtEntryAssemblyConfig");
        var txtCallingAssemblyConfig = this.FindControl<TextBlock>("TxtCallingAssemblyConfig");
        _txtMainThemeConfig = this.FindControl<TextBlock>("TxtMainThemeConfig");
        _txtChildThemeConfig = this.FindControl<TextBlock>("TxtChildThemeConfig");

        txtEntryAssemblyConfig!.Text = AppConfigHelper.GetEntryAssemblyConfigPath();
        txtCallingAssemblyConfig!.Text = AppConfigHelper.GetCallingAssemblyConfigPath();
        _entryAssemblyConfigHelper = AppConfigHelper.GetEntryAssembly();
        _CallingAssemblyConfigHelper = AppConfigHelper.GetCallingAssembly();
    }

    private void ReadMainConfig_OnClick(object sender, RoutedEventArgs e)
    {
        _txtMainThemeConfig!.Text = _entryAssemblyConfigHelper.Get<string>(ThemeKey);
    }

    private void ChangeMainConfig_OnClick(object sender, RoutedEventArgs e)
    {
        if (_entryAssemblyConfigHelper.Get<string>(ThemeKey) == "Dark")
        {
            _entryAssemblyConfigHelper.Set(ThemeKey, "Light");
        }
        else
        {
            _entryAssemblyConfigHelper.Set(ThemeKey, "Dark");
        }
        _txtMainThemeConfig!.Text = _entryAssemblyConfigHelper.Get<string>(ThemeKey);
    }

    private void ReadChildProjectThemeConfig_OnClick(object sender, RoutedEventArgs e)
    {
        _txtChildThemeConfig!.Text = _CallingAssemblyConfigHelper.Get<string>(ThemeKey);
    }

    private void ChangeChildProjectThemeConfig_OnClick(object sender, RoutedEventArgs e)
    {
        if (_CallingAssemblyConfigHelper.Get<string>(ThemeKey) == "Dark")
        {
            _CallingAssemblyConfigHelper.Set(ThemeKey, "Light");
        }
        else
        {
            _CallingAssemblyConfigHelper.Set(ThemeKey, "Dark");
        }
        _txtChildThemeConfig!.Text = _CallingAssemblyConfigHelper.Get<string>(ThemeKey);
    }
}