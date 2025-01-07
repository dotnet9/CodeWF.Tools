using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CodeWF.Tools.AvaloniaDemo.Models;
using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;

namespace CodeWF.Tools.AvaloniaDemo.Views;

public partial class MainView : UserControl
{
    private readonly TextBlock? _txtMainThemeConfig;
    private const string ThemeKey = "Theme";

    public MainView()
    {
        InitializeComponent();
        _txtMainThemeConfig = this.FindControl<TextBlock>("TxtMainThemeConfig");
    }

    private void ReadMainConfig_OnClick(object sender, RoutedEventArgs e)
    {
        _txtMainThemeConfig!.Text = AppConfigHelper.TryGet<string>(ThemeKey, out var theme) ? theme : "No config";
    }

    private void ReadSpecialConfig_OnClick(object sender, RoutedEventArgs e)
    {
        var config = AppConfigHelper.OpenConfig(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "CodeWF.Tools.AvaloniaDemo.config"));
        config.TryGet("Theme", out string? theme);
        _txtMainThemeConfig!.Text = theme;
    }

    private void ChangeMainConfig_OnClick(object sender, RoutedEventArgs e)
    {
        if (AppConfigHelper.TryGet<string>(ThemeKey, out var theme) && theme == nameof(ThemeVariant.Dark))
        {
            AppConfigHelper.Set(ThemeKey, nameof(ThemeVariant.Light));
        }
        else
        {
            AppConfigHelper.Set(ThemeKey, nameof(ThemeVariant.Dark));
        }

        ReadMainConfig_OnClick(sender, e);
    }

    private void Json2Yaml_OnClick(object sender, RoutedEventArgs e)
    {
        var obj = School.GetStudent();

        obj.ToJson(out var jsonString, out var errorMsg);
        jsonString.JsonToYaml(out var yamlString, out errorMsg);
    }

    private void Yaml2Json_OnClick(object sender, RoutedEventArgs e)
    {
        var obj = School.GetStudent();

        obj.ToYaml(out var yamlString, out var errorMsg);
        yamlString.YamlToJson(out var jsonString, out errorMsg);
    }
}