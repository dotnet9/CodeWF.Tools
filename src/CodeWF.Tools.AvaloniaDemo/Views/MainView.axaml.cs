using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CodeWF.Log.Core;
using CodeWF.Tools.AvaloniaDemo.Models;
using CodeWF.Tools.Extensions;
using CodeWF.Tools.FileExtensions;
using CodeWF.Tools.Helpers;
using CodeWF.Tools.Image;

namespace CodeWF.Tools.AvaloniaDemo.Views;

public partial class MainView : UserControl
{
    private const string ThemeKey = "Theme";
    private const string DemoFolderName = "CodeWF.Tools.AvaloniaDemo";

    public MainView()
    {
        InitializeComponent();

        // Demo 程序统一通过 UI 日志和本地文件记录测试过程，避免控制台窗口噪声。
        Logger.EnableConsoleOutput = false;
        LogInfo("CodeWF.Tools 桌面测试台已启动");
    }

    private void SelectConfigTab_OnClick(object? sender, RoutedEventArgs e)
    {
        ToolTabs.SelectedIndex = 0;
    }

    private void SelectSerializationTab_OnClick(object? sender, RoutedEventArgs e)
    {
        ToolTabs.SelectedIndex = 1;
    }

    private void SelectFileNetworkTab_OnClick(object? sender, RoutedEventArgs e)
    {
        ToolTabs.SelectedIndex = 2;
    }

    private async void RunAllTests_OnClick(object? sender, RoutedEventArgs e)
    {
        await RunStepAsync("配置读写", async () => TxtMainThemeConfig.Text = await ExecuteConfigTestAsync());
        await RunStepAsync("通配正则", () => TxtRegexResult.Text = ExecuteRegexTest());
        await RunStepAsync("超时检测", () => TxtRegexResult.Text += Environment.NewLine + ExecuteActionHelperTest());
        await RunStepAsync("程序集信息", () => TxtRegexResult.Text += Environment.NewLine + ExecuteAssemblyInfoTest());
        await RunStepAsync("JSON 与 YAML", async () => TxtJsonStr.Text = await ExecuteJsonYamlTestAsync());
        await RunStepAsync("模型序列化", () => TxtXmlStr.Text = ExecuteSerializationTest());
        await RunStepAsync("文件帮助方法", async () => TxtFileResult.Text = await ExecuteFileHelperTestAsync());
        await RunStepAsync("路径标准化", () => TxtFileResult.Text += Environment.NewLine + ExecutePathHelperTest());
        await RunStepAsync("网络帮助方法", async () => TxtNetworkResult.Text = await ExecuteNetworkHelperTestAsync());
        await RunStepAsync("组播地址生成", () => TxtNetworkResult.Text += Environment.NewLine + ExecuteMulticastHelperTest());
        await RunStepAsync("二维码生成", () => TxtImageResult.Text = ExecuteQrCodeTest());
        await RunStepAsync("ICO 生成", async () => TxtImageResult.Text += Environment.NewLine + await ExecuteIconHelperTestAsync());
        SetStatus("全部测试已完成");
    }

    private void ClearOutput_OnClick(object? sender, RoutedEventArgs e)
    {
        foreach (var output in GetOutputControls())
        {
            output.Text = string.Empty;
        }

        SetStatus("已清空测试结果");
        LogInfo("已清空界面测试结果");
    }

    private async void ReadMainConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtMainThemeConfig.Text = await ExecuteConfigReadTestAsync(AppConfigHelper.GetDefaultConfigPath(), "主配置");
    }

    private async void ReadSpecialConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "CodeWF.Tools.AvaloniaDemo.config");
        TxtMainThemeConfig.Text = await ExecuteConfigReadTestAsync(configPath, "指定配置");
    }

    private async void ChangeMainConfig_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtMainThemeConfig.Text = await ExecuteConfigTestAsync();
    }

    private async void Json2Yaml_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtJsonStr.Text = await ExecuteJsonToYamlTestAsync();
    }

    private void Yaml2Json_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtJsonStr.Text = ExecuteYamlToJsonTest();
    }

    private void SerialTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtXmlStr.Text = ExecutePenListSerializationTest();
    }

    private void SerialNormalClass_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtXmlStr.Text = ExecuteProjectSerializationTest();
    }

    private void SerialClassWithEnum_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtXmlStr.Text = ExecuteEnumSerializationTest();
    }

    private void SerialDictionary_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtXmlStr.Text = ExecuteDictionarySerializationTest();
    }

    private void SerialClassWithDict_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtJsonStr.Text = ExecuteStudentRoundTripTest();
    }

    private void SerialClassXml_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtXmlStr.Text = ExecuteXmlSerializationTest();
    }

    private void RegexTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtRegexResult.Text = ExecuteRegexTest();
    }

    private void ActionHelperTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtRegexResult.Text = ExecuteActionHelperTest();
    }

    private void AssemblyInfoTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtRegexResult.Text = ExecuteAssemblyInfoTest();
    }

    private async void FileHelperTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtFileResult.Text = await ExecuteFileHelperTestAsync();
    }

    private void PathHelperTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtFileResult.Text = ExecutePathHelperTest();
    }

    private async void NetworkHelperTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtNetworkResult.Text = await ExecuteNetworkHelperTestAsync();
    }

    private void MulticastHelperTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtNetworkResult.Text = ExecuteMulticastHelperTest();
    }

    private void GeneratorQrCode_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtImageResult.Text = ExecuteQrCodeTest();
    }

    private async void IconHelperTest_OnClick(object? sender, RoutedEventArgs e)
    {
        TxtImageResult.Text = await ExecuteIconHelperTestAsync();
    }

    private async Task<string> ExecuteConfigTestAsync()
    {
        var mainConfigPath = AppConfigHelper.GetDefaultConfigPath();
        AppConfigHelper.TryGet(mainConfigPath, ThemeKey, out string? oldTheme);
        var nextTheme = oldTheme == nameof(ThemeVariant.Dark)
            ? nameof(ThemeVariant.Light)
            : nameof(ThemeVariant.Dark);
        AppConfigHelper.Set(mainConfigPath, ThemeKey, nextTheme);

        var readBack = AppConfigHelper.TryGet(mainConfigPath, ThemeKey, out string? newTheme);
        var specialPath = Path.Combine(AppContext.BaseDirectory, "CodeWF.Tools.AvaloniaDemo.config");
        var specialRead = AppConfigHelper.TryGet(specialPath, ThemeKey, out string? specialTheme);
        await Task.CompletedTask;

        var result = $"""
                     主配置路径：{mainConfigPath}
                     原主题值：{oldTheme ?? "未设置"}
                     新主题值：{(readBack ? newTheme : "读取失败")}
                     指定配置路径：{specialPath}
                     指定配置主题：{(specialRead ? specialTheme : "读取失败")}
                     """;
        LogInfo("App.config 读写测试完成");
        SetStatus("配置读写测试完成");
        return result;
    }

    private async Task<string> ExecuteConfigReadTestAsync(string configPath, string configName)
    {
        var success = AppConfigHelper.TryGet(configPath, ThemeKey, out string? theme);
        await Task.CompletedTask;
        var result = $"""
                     {configName}路径：{configPath}
                     读取结果：{(success ? "成功" : "失败")}
                     Theme：{theme ?? "未设置"}
                     """;
        LogInfo($"{configName}读取测试完成，结果：{(success ? "成功" : "失败")}");
        SetStatus($"{configName}读取完成");
        return result;
    }

    private async Task<string> ExecuteJsonYamlTestAsync()
    {
        var jsonToYaml = await ExecuteJsonToYamlTestAsync();
        var yamlToJson = ExecuteYamlToJsonTest();
        return $"{jsonToYaml}{Environment.NewLine}{Environment.NewLine}{yamlToJson}";
    }

    private async Task<string> ExecuteJsonToYamlTestAsync()
    {
        var student = School.ManualMockStudent();
        await Task.CompletedTask;

        if (!student.ToJson(SourceGenerationContext.Default.Student, out var jsonString, out var errorMsg))
        {
            return $"JSON 序列化失败：{errorMsg}";
        }

        if (!jsonString.JsonToYaml(out var yamlString, out errorMsg))
        {
            return $"JSON 转 YAML 失败：{errorMsg}";
        }

        LogInfo("JSON 转 YAML 测试完成");
        SetStatus("JSON 转 YAML 完成");
        return $"JSON 转 YAML 成功：{Environment.NewLine}{yamlString}";
    }

    private string ExecuteYamlToJsonTest()
    {
        const string yamlString = """
                                  Name: Liu
                                  Gender: Male
                                  Year: 35
                                  CreateTime: 2026-05-09T00:00:00
                                  Tags:
                                  - Math
                                  - Science
                                  """;

        if (!yamlString.YamlToJson(out var jsonString, out var errorMsg))
        {
            return $"YAML 转 JSON 失败：{errorMsg}";
        }

        LogInfo("YAML 转 JSON 测试完成");
        SetStatus("YAML 转 JSON 完成");
        return $"YAML 转 JSON 成功：{Environment.NewLine}{jsonString}";
    }

    private string ExecuteSerializationTest()
    {
        var results = new[]
        {
            ExecutePenListSerializationTest(),
            ExecuteProjectSerializationTest(),
            ExecuteEnumSerializationTest(),
            ExecuteDictionarySerializationTest(),
            ExecuteStudentRoundTripTest(),
            ExecuteXmlSerializationTest()
        };
        LogInfo("模型序列化覆盖测试完成");
        return string.Join($"{Environment.NewLine}{Environment.NewLine}", results);
    }

    private string ExecutePenListSerializationTest()
    {
        List<Pen> data =
        [
            // 使用中文数据验证编码、JSON 源生成和 JsonIgnore 行为。
            new("蓝色", 0.5, "圆珠笔", true),
            new("黑色", 0.7, "中性笔", true),
            new("红色", 1.0, "马克笔", false),
            new("绿色", 0.3, "钢笔", true)
        ];

        return data.ToJson(SourceGenerationContext.Default.ListPen, out var json, out var errorMsg)
            ? $"列表模型序列化成功：{Environment.NewLine}{json}"
            : $"列表模型序列化失败：{errorMsg}";
    }

    private string ExecuteProjectSerializationTest()
    {
        List<Project> data =
        [
            new() { Id = 1, Name = "项目一", Record = 100 },
            new() { Id = null, Name = "项目二", Record = 88 },
            new() { Id = 3, Name = "项目三", Record = 95 }
        ];

        return data.ToJson(SourceGenerationContext.Default.ListProject, out var json, out var errorMsg)
            ? $"普通模型序列化成功：{Environment.NewLine}{json}"
            : $"普通模型序列化失败：{errorMsg}";
    }

    private string ExecuteEnumSerializationTest()
    {
        List<ClassWithEnum> data =
        [
            new() { Id = 1, Name = "浅色", Type = TestEnum.Light },
            new() { Id = 2, Name = "深色", Type = TestEnum.Dark }
        ];

        return data.ToJson(SourceGenerationContext.Default.ListClassWithEnum, out var json, out var errorMsg)
            ? $"枚举模型序列化成功：{Environment.NewLine}{json}"
            : $"枚举模型序列化失败：{errorMsg}";
    }

    private static string ExecuteDictionarySerializationTest()
    {
        var data = new Dictionary<string, double>
        {
            ["math"] = 100.0,
            ["english"] = 99.5
        };
        var json = JsonSerializer.Serialize(data, SourceGenerationContext.Default.DictionaryStringDouble);
        return $"字典模型序列化成功：{Environment.NewLine}{json}";
    }

    private static string ExecuteStudentRoundTripTest()
    {
        var data = School.ManualMockStudent();
        if (!data.ToJson(SourceGenerationContext.Default.Student, out var json, out var errorMsg))
        {
            return $"复杂对象序列化失败：{errorMsg}";
        }

        if (!json.FromJson(SourceGenerationContext.Default.Student, out var desObj, out errorMsg))
        {
            return $"复杂对象反序列化失败：{errorMsg}";
        }

        return $"""
               复杂对象往返成功：
               学生姓名：{desObj?.Name}
               项目数量：{desObj?.Projects?.Count}
               创建时间：{desObj?.CreateTime:yyyy-MM-dd HH:mm:ss}
               """;
    }

    private string ExecuteXmlSerializationTest()
    {
        var document = BuildProjectXml(BuildProjectData());
        var filePath = Path.Combine(GetDemoOutputFolder(), "project.xml");
        document.Save(filePath);
        LogInfo($"XML 示例已保存：{filePath}");
        SetStatus("XML 示例生成完成");
        return $"XML 输出成功：{filePath}{Environment.NewLine}{document}";
    }

    private static Project BuildProjectData()
    {
        return new Project
        {
            Id = 1,
            Name = "CodeWF.Tools 测试项目",
            Record = 100,
            Members =
            [
                new Member { Name = "张三", Age = 30 },
                new Member { Name = "李四", Age = 25 }
            ]
        };
    }

    private static XDocument BuildProjectXml(Project projectData)
    {
        return new XDocument(
            new XElement(
                nameof(Project),
                new XElement(nameof(Project.Id), projectData.Id),
                new XElement(nameof(Project.Name), projectData.Name),
                new XElement(nameof(Project.Record), projectData.Record),
                new XElement(
                    nameof(Project.Members),
                    projectData.Members.ConvertAll(member =>
                        new XElement(
                            nameof(Member),
                            new XElement(nameof(Member.Name), member.Name),
                            new XElement(nameof(Member.Age), member.Age))))));
    }

    private string ExecuteRegexTest()
    {
        var cases = new[]
        {
            ("GuardianX", "Guard*"),
            ("订单A123", "订单@###"),
            ("版本2026", "*2026"),
            ("alpha-42", "alpha-##,beta-##")
        };

        var builder = new StringBuilder();
        foreach (var (input, pattern) in cases)
        {
            var converted = pattern.ConvertPattern();
            var isMatch = input.IsMatch(pattern);
            builder.AppendLine($"输入：{input}，模式：{pattern}，转换后：{converted}，结果：{(isMatch ? "匹配" : "不匹配")}");
        }

        LogInfo("通配正则测试完成");
        SetStatus("通配正则测试完成");
        return builder.ToString();
    }

    private string ExecuteActionHelperTest()
    {
        var fastResult = ActionHelper.CheckOvertime(() => Thread.Sleep(50), 500);
        var slowResult = ActionHelper.CheckOvertime(() => Thread.Sleep(300), 50);
        LogInfo("超时检测测试完成");
        SetStatus("超时检测测试完成");
        return $"""
               超时检测：
               50ms 操作 / 500ms 超时：{(fastResult ? "按时完成" : "超时")}
               300ms 操作 / 50ms 超时：{(slowResult ? "按时完成" : "超时")}
               """;
    }

    private static string ExecuteAssemblyInfoTest()
    {
        var assembly = App.ExecutingAssembly ?? Assembly.GetExecutingAssembly();
        return $"""
               程序集信息：
               标题：{assembly.Title() ?? "未设置"}
               产品：{assembly.Product() ?? "未设置"}
               版本：{assembly.Version() ?? "未设置"}
               文件版本：{assembly.FileVersion() ?? "未设置"}
               编译时间：{assembly.CompileTime()?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未知"}
               """;
    }

    private async Task<string> ExecuteFileHelperTestAsync()
    {
        var filePath = FileHelper.GetTempFileName(DemoFolderName);
        const string content = "CodeWF.Tools 文件读写测试：中文简体日志与 UTF-8 内容。";
        await FileHelper.SafeWriteAllTextAsync(filePath, content, Encoding.UTF8);
        var readText = await FileHelper.SafeReadAllTextAsync(filePath, Encoding.UTF8);
        var encodingName = FileHelper.GetFileEncodeType(filePath).EncodingName;
        await FileHelper.DeleteFileIfExist(filePath);

        LogInfo("文件读写与编码识别测试完成");
        SetStatus("文件帮助方法测试完成");
        return $"""
               临时文件：{filePath}
               读取内容：{readText}
               识别编码：{encodingName}
               删除结果：{(!File.Exists(filePath) ? "已删除" : "仍存在")}
               """;
    }

    private string ExecutePathHelperTest()
    {
        const string mixedPath = @"C:/Temp/CodeWF//Tools/demo.txt";
        var normalizedPath = FileHelper.NormalizePathSeparators(mixedPath);
        LogInfo("路径标准化测试完成");
        SetStatus("路径标准化测试完成");
        return $"""
               原始路径：{mixedPath}
               标准路径：{normalizedPath}
               """;
    }

    private async Task<string> ExecuteNetworkHelperTestAsync()
    {
        var ips = await IpHelper.GetAllIpV4Async();
        var port = IpHelper.GetAvailableTcpPort();
        LogInfo("本机地址与可用端口测试完成");
        SetStatus("网络帮助方法测试完成");
        return $"""
               IPv4 地址：{(ips.Count > 0 ? string.Join(", ", ips) : "未发现")}
               可用 TCP 端口：{port}
               """;
    }

    private string ExecuteMulticastHelperTest()
    {
        IpHelper.GetMulticastIpAndPort(out var ip, out var port, 7200, 7210, needConnectCheck: false);
        LogInfo("组播地址生成测试完成");
        SetStatus("组播地址生成测试完成");
        return $"""
               组播地址：{ip}
               组播端口：{port}
               """;
    }

    private string ExecuteQrCodeTest()
    {
        var filePath = Path.Combine(GetDemoOutputFolder(), "codewf-tools-demo.png");
        QrCodeGenerator.GenerateQrCode(
            "测试",
            "https://codewf.com",
            filePath,
            "CodeWF.Tools 桌面测试程序生成");

        LogInfo($"二维码已生成：{filePath}");
        SetStatus("二维码生成完成");
        return $"二维码生成成功：{filePath}";
    }

    private async Task<string> ExecuteIconHelperTestAsync()
    {
        var folder = GetDemoOutputFolder();
        var sourcePath = Path.Combine(folder, "codewf-tools-demo.png");
        if (!File.Exists(sourcePath))
        {
            QrCodeGenerator.GenerateQrCode("测试", "https://codewf.com", sourcePath, "CodeWF.Tools 桌面测试程序生成");
        }

        var iconPath = Path.Combine(folder, "codewf-tools-demo.ico");
        await CodeWF.Tools.ImageHelper.MergeGenerateIcon(sourcePath, iconPath, [16, 32, 64, 128]);
        LogInfo($"ICO 文件已生成：{iconPath}");
        SetStatus("ICO 生成完成");
        return $"ICO 生成成功：{iconPath}";
    }

    private async Task RunStepAsync(string name, Func<Task> action)
    {
        try
        {
            SetStatus($"正在执行：{name}");
            LogInfo($"开始执行：{name}");
            await action();
            LogInfo($"执行成功：{name}");
        }
        catch (Exception ex)
        {
            LogError($"执行失败：{name}", ex);
            SetStatus($"{name} 失败：{ex.Message}");
        }
    }

    private Task RunStepAsync(string name, Action action)
    {
        return RunStepAsync(
            name,
            () =>
            {
                action();
                return Task.CompletedTask;
            });
    }

    private IEnumerable<TextBox> GetOutputControls()
    {
        yield return TxtMainThemeConfig;
        yield return TxtRegexResult;
        yield return TxtJsonStr;
        yield return TxtXmlStr;
        yield return TxtFileResult;
        yield return TxtNetworkResult;
        yield return TxtImageResult;
    }

    private static string GetDemoOutputFolder()
    {
        var folder = Path.Combine(Path.GetTempPath(), DemoFolderName, "Outputs");
        Directory.CreateDirectory(folder);
        return folder;
    }

    private void SetStatus(string message)
    {
        TxtStatus.Text = message;
    }

    private static void LogInfo(string message)
    {
        Logger.Info(message, message, log2UI: true, log2File: true, log2Console: false);
    }

    private static void LogError(string message, Exception exception)
    {
        var uiMessage = $"{message}：{exception.Message}";
        Logger.Error(uiMessage, exception, uiMessage, log2UI: true, log2File: true, log2Console: false);
    }
}

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, WriteIndented = true)]
[JsonSerializable(typeof(Dictionary<string, double>))]
[JsonSerializable(typeof(List<Pen>))]
[JsonSerializable(typeof(List<Project>))]
[JsonSerializable(typeof(List<ClassWithEnum>))]
[JsonSerializable(typeof(Student))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
