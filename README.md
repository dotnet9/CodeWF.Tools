# CodeWF.Tools

| 包名 | NuGet 链接 | 下载量 |
|------|-----------|--------|
| CodeWF.Tools | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools)](https://www.nuget.org/packages/CodeWF.Tools/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools)](https://www.nuget.org/packages/CodeWF.Tools/) |
| CodeWF.Tools.Core | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools.Core.svg)](https://www.nuget.org/packages/CodeWF.Tools.Core/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools.Core.svg)](https://www.nuget.org/packages/CodeWF.Tools.Core/) |
| CodeWF.Tools.Files | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools.Files.svg)](https://www.nuget.org/packages/CodeWF.Tools.Files/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools.Files.svg)](https://www.nuget.org/packages/CodeWF.Tools.Files/) |
| CodeWF.Tools.Image | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools.Image.svg)](https://www.nuget.org/packages/CodeWF.Tools.Image/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools.Image.svg)](https://www.nuget.org/packages/CodeWF.Tools.Image/) |

**CodeWF.Tools，让 C# 编码变得更简单。**

CodeWF.Tools 是一个面向 C# 开发者的开源工具库，提供字符串、日期时间、文件操作、JSON/YAML 转换、图片处理等常用能力，目标是简化日常开发任务并提升开发效率。

- CodeWF.Tools.Core: .NET原生功能扩展，无第三方库依赖
- CodeWF.Tools.Files：文件操作类集合
- CodeWF.Tools.Image：图片处理集合
- CodeWF.Tools：集成前面的包，功能最全

## 仓库规范

- 当前版本：`1.3.13.5`，版本号统一维护在根目录 `Directory.Build.props` 的 `<Version>` 节点。
- NuGet 包项目统一支持 `net8.0;net10.0`；Demo、App、测试与内部应用项目统一使用 `net11.0` / `net11.0-windows`。
- 根目录 `logo.svg`、`logo.png`、`logo.ico` 是唯一图标源，子工程只通过 MSBuild `Link` 引用，不维护图标副本。
- 运行时帮助、Markdown 示例、内置备忘录、设计说明等业务文档按功能保留；仓库级入口文档使用根目录 `README.md` 和 `UpdateLog.md`。

## 开发

本仓库使用 NuGet 中央包管理，包版本统一维护在 `Directory.Packages.props`。

- 更新日志：[UpdateLog.md](UpdateLog.md)
- 贡献指南：[CONTRIBUTING.md](CONTRIBUTING.md)
- 安全策略：[SECURITY.md](SECURITY.md)
- 一键打包：运行 `pack.bat`，NuGet 包输出到 `artifacts\packages`。

## 感谢

- Masuit.Tools：https://github.com/ldqk/Masuit.Tools

## 第三方开源组件审计

检查时间：2026-05-20。检查范围包括 NuGet 元数据、恢复后的 `project.assets.json`、NuGet.org 信息以及上游源码/许可证链接。优先接受 MIT / Apache-2.0 / BSD。

| 包 | 使用范围 | 协议 | 源码/项目地址 | 结论 |
| --- | --- | --- | --- | --- |
| `CsvHelper` | `CodeWF.Tools.Files` CSV 辅助 | MS-PL OR Apache-2.0 | https://github.com/JoshClose/CsvHelper | 通过，采用 Apache-2.0 选项 |
| `Magick.NET-Q16-AnyCPU` | `CodeWF.Tools.Image` 图片辅助 | Apache-2.0 | https://github.com/dlemstra/Magick.NET | 通过，`14.13.1` |
| `MiniExcel` | `CodeWF.Tools.Files` Excel 辅助 | Apache-2.0 | https://github.com/mini-software/MiniExcel | 通过，`1.44.1` |
| `SharpCompress` | `CodeWF.Tools.Files` 压缩包辅助 | MIT | https://github.com/adamhathcock/sharpcompress | 通过，`1.0.0` |
| `System.Configuration.ConfigurationManager` / `System.Text.Json` | 配置和 JSON 辅助 | MIT | https://github.com/dotnet/dotnet | 通过，`10.0.8` |
| `YamlDotNet` | `CodeWF.Tools.Files` YAML 辅助 | MIT | https://github.com/aaubry/YamlDotNet | 通过 |
| `ZXing.Net.Bindings.Magick` | 二维码/条码辅助 | Apache-2.0 | https://github.com/micjahn/ZXing.Net | 通过 |
| `Avalonia` / `Avalonia.Desktop` / `Avalonia.Fonts.Inter` / `Avalonia.Themes.Fluent` | Demo 使用 | MIT | https://github.com/AvaloniaUI/Avalonia | 通过，`12.0.3` |
| `Bogus` | 测试数据 | MIT | https://github.com/bchavez/Bogus | 通过 |
| `CodeWF.LogViewer.Avalonia` | Demo 使用 | MIT | https://github.com/dotnet9/CodeWF.LogViewer | 自研开源包 |
| `ReactiveUI.Avalonia` | Demo 使用 | MIT | https://github.com/reactiveui/reactiveui | 通过 |
| `VC-LTL` | Windows Demo 运行时兼容 | EPL-2.0 | https://github.com/Chuyu-Team/VC-LTL5 | 源码开放，按“非优先但可追溯”规则通过 |
| `Microsoft.NET.Test.Sdk` / `coverlet.collector` | 测试 | MIT | https://github.com/microsoft/vstest / https://github.com/coverlet-coverage/coverlet | 通过，`Microsoft.NET.Test.Sdk` `18.5.1`、`coverlet.collector` `10.0.1` |
| `xunit.v3` / `xunit.runner.visualstudio` | 测试 | Apache-2.0 | https://github.com/xunit/xunit | 通过 |

传递依赖检查：

| 依赖分组 | 代表包 | 协议 | 源码/项目地址 | 结论 |
| --- | --- | --- | --- | --- |
| .NET 运行时/配置栈 | `System.Diagnostics.EventLog`、`System.IO.Pipelines`、`System.Text.Encodings.Web`、`System.Security.Cryptography.ProtectedData` | MIT | https://github.com/dotnet/dotnet | 源码开放 |
| Magick.NET 核心 | `Magick.NET.Core` | Apache-2.0 | https://github.com/dlemstra/Magick.NET | 源码开放 |
| ZXing 核心 | `ZXing.Net` | Apache-2.0 | https://github.com/micjahn/ZXing.Net | 源码开放 |
| Avalonia 栈 | `Avalonia.*`、`Avalonia.BuildServices` | MIT | https://github.com/AvaloniaUI/Avalonia | 源码开放 |
| ANGLE 原生包 | `Avalonia.Angle.Windows.Natives` | BSD 风格许可证文件 | https://github.com/AvaloniaUI/angle | 源码开放 |
| Skia/HarfBuzz 绑定和原生资源 | `SkiaSharp*`、`HarfBuzzSharp*` | NuGet 包为 MIT；底层原生项目源码开放 | https://github.com/mono/SkiaSharp | 源码开放 |
| Reactive 栈 | `ReactiveUI`、`DynamicData`、`Splat`、`System.Reactive` | MIT | https://github.com/reactiveui/reactiveui / https://github.com/reactiveui/DynamicData / https://github.com/reactiveui/splat / https://github.com/dotnet/reactive | 源码开放 |
| Linux 桌面互操作 | `MicroCom.Runtime`、`Tmds.DBus.Protocol` | MIT | https://github.com/kekekeks/MicroCom / https://github.com/tmds/Tmds.DBus | 源码开放 |
| 自研日志依赖 | `CodeWF.Log.Core` | MIT | https://github.com/dotnet9/CodeWF.LogViewer | 自研源码开放包 |
| 测试工具链 | `Microsoft.*`、`Newtonsoft.Json`、`xunit.*` | MIT / Apache-2.0 | https://github.com/microsoft/vstest / https://github.com/microsoft/testfx / https://github.com/JamesNK/Newtonsoft.Json / https://github.com/xunit/xunit | 源码开放 |

当前未有意保留非开源或黑盒依赖。
## Package Versioning Convention

Keep NuGet package versions and Central Package Management settings in `Directory.Packages.props`, including shared version properties such as `AvaloniaVersion`. Keep `Directory.Build.props` focused on build, compiler, and NuGet package metadata. When referenced, `VC-LTL` and `YY-Thunks` should use their latest prerelease versions for OS platform compatibility.
