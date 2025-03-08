using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Bogus.DataSets;
using CodeWF.Tools.AvaloniaDemo.Models;
using CodeWF.Tools.Extensions;
using CodeWF.Tools.Helpers;
using CodeWF.Tools.Image;
using System;
using System.Collections.Generic;

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

    private void SerialNormalClass_OnClick(object? sender, RoutedEventArgs e)
    {
        List<Project> data = new()
        {
            new Project() { Id = "1", Name = "n1", Record = 1 },
            new Project() { Id = "1", Name = "n1", Record = 1 },
            new Project() { Id = "1", Name = "n1", Record = 1 }
        };
        data.ToJson(out var json, out var errorMsg);
        TxtJsonStr.Text = json;
    }

    private void SerialClassWithEnum_OnClick(object? sender, RoutedEventArgs e)
    {
        List<ClassWithEnum> data = new()
        {
            new ClassWithEnum() { Id = 1, Name = "N1", Type = TestEnum.Dark },
            new ClassWithEnum() { Id = 1, Name = "N1", Type = TestEnum.Light }
        };
        if (data.ToJson(out var json, out var errorMsg))
        {
            TxtJsonStr.Text = json;
        }
        else
        {
            TxtJsonStr.Text = $"序列化异常：{errorMsg}";
        }
    }

    private void GeneratorQrCode_OnClick(object? sender, RoutedEventArgs e)
    {
        var title = "微信扫码，通知挪车";

        var content =
            $"https://codewf.com/qr/16800000000";
        var ad = "码坊：https://codewf.com";
        var savePath = "nuoche.png";

        QrCodeGenerator.GenerateQrCode(title, ad, content, savePath);
        Console.WriteLine("图片已生成并保存到：" + savePath);
    }
}