using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CodeWF.Tools.AvaloniaDemo.Models;
using CodeWF.Tools.Extensions;
using CodeWF.Tools.FileExtensions;
using CodeWF.Tools.Helpers;
using CodeWF.Tools.Image;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

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
        var obj = School.ManualMockStudent();

        obj.ToJson(out var jsonString, out var errorMsg);
        jsonString.JsonToYaml(out var yamlString, out errorMsg);
    }

    private void Yaml2Json_OnClick(object sender, RoutedEventArgs e)
    {
        var obj = School.ManualMockStudent();

        obj.ToYaml(out var yamlString, out var errorMsg);
        yamlString.YamlToJson(out var jsonString, out errorMsg);
    }

    private void SerialTest_OnClick(object? sender, RoutedEventArgs e)
    {
        List<Pen> data =
        [
            // 添加几支不同的笔到集合中
            new("蓝色", 0.5, "圆珠笔", true),
            new("黑色", 0.7, "中性笔", true),
            new("红色", 1.0, "马克笔", false),
            new("绿色", 0.3, "钢笔", true),
        ];
        TxtJsonStr.Text = data.ToJson(out var json, out var errorMsg) ? json : $"序列化异常：{errorMsg}";
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

    private void SerialDictionary_OnClick(object? sender, RoutedEventArgs e)
    {
        var data = new Dictionary<string, double>();
        data["math"] = 100.0;
        data["english"] = 99.5;
        var json = JsonSerializer.Serialize(data, SourceGenerationContext.Default.DictionaryStringDouble);
        TxtJsonStr.Text = json;
        //if (data.ToJson(out var json,  out var errorMsg))
        //{
        //    TxtJsonStr.Text = json;
        //}
        //else
        //{
        //    TxtJsonStr.Text = $"序列化异常：{errorMsg}";
        //}
    }

    private void SerialClassWithDict_OnClick(object? sender, RoutedEventArgs e)
    {
        var data = School.ManualMockStudent();
        TxtJsonStr.Text = data.ToJson(out var json, out var errorMsg) ? json : $"序列化异常：{errorMsg}";

        json.FromJson<Student>(out var desObj, out errorMsg);
        TxtJsonStr.Text += $"反序列化后时间：{desObj?.CreateTime}";
    }

    private void SerialClassXml_OnClick(object? sender, RoutedEventArgs e)
    {
        var projectData = new Project
        {
            Id = "Math",
            Name = "Software Engineer",
            Record = 100,
            Members =
            [
                new Member { Name = "Alice", Age = 30 },
                new Member { Name = "Bob", Age = 25 }
            ]
        };
        try
        {
            XmlSerializer xz = new XmlSerializer(typeof(Project));
            using var stream = new StreamWriter("D://test.xml", false, Encoding.UTF8);
            xz.Serialize(stream, projectData);
        }
        catch (Exception ex)
        {
            TxtXmlStr.Text = $"序列化异常：{ex}";
        }
    }

    private void GeneratorQrCode_OnClick(object? sender, RoutedEventArgs e)
    {
        var title = "扫码挪车";
        var subTitle = "扫码联系车主或拨打电话: 16800000000";
        var content =
            $"https://codewf.com/nuoche?p=16800000000";
        var savePath = "nuoche.png";

        QrCodeGenerator.GenerateQrCode(title, content, savePath, subTitle);
        FileHelper.OpenFolderAndSelectFile(savePath);
        Console.WriteLine("图片已生成并保存到：" + savePath);
    }
}

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(Dictionary<string, double>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}