# 更新日志

## 1.3.13.5 (2026-06-08)

- 🎨[优化]-重新设计根目录 `logo.svg`、`logo.png`、`logo.ico`，使用透明底工具箱主体图形表达 C# 常用工具集合定位，提升任务栏、浏览器标签和 NuGet 小图标辨识度。
- 🔨[优化]-修复单独执行 `pack.bat` 时项目级 `dotnet pack` 的中间目录解析问题，并避免构建阶段提前将 NuGet 包输出到 `Output`。

## 1.3.13.4 (2026-06-08)

- 🔨[优化]-补齐根目录 logo.svg、logo.png、logo.ico 三件套，子工程通过 MSBuild Link 引用根 logo，避免维护多份图标副本。
- 🔨[优化]-统一目标框架：NuGet 包项目支持 `net8.0;net10.0`，Demo、App、测试与内部应用项目升级到 `net11.0` / `net11.0-windows`。
- 🔨[优化]-保留运行时帮助、Markdown 示例、内置备忘录和业务设计文档，仅收敛仓库级重复文档入口。

## 1.3.13.3 (2026-06-08)

- 统一版本号维护入口，只在仓库根目录 `Directory.Build.props` 中定义 `<Version>`。
- 清理英文/双语文档入口，后续仅维护简体中文文档。
- 完善 NuGet 发布配置，补充 Source Link、符号包和标签格式规范。


## V1.3.13.2（2026-05-20）

- 🔨[优化]-包版本提升到 `1.3.13.2`。
- 🔨[优化]-Avalonia Demo 相关包更新到 `Avalonia` 12.0.3。
- 🔨[优化]-升级到本仓库已检查的源码开放 NuGet 稳定最新版：`coverlet.collector` 10.0.1、`MiniExcel` 1.44.1、`SharpCompress` 1.0.0、`System.Configuration.ConfigurationManager` 10.0.8、`System.Text.Json` 10.0.8。
- 🔨[优化]-启用 NuGet 中央传递依赖固定，使依赖解析更严格。
- 🔨[优化]-适配 SharpCompress 1.0 的非泛型 `IWritableArchive` 与 `ArchiveFactory.Create(ArchiveType.Zip)` API。

## V1.3.12.3（2026-05-09）

- 😄[新增]-新增 `Directory.Packages.props`，支持 NuGet 中央包管理。
- 🔨[优化]-升级到当前免费/开源 NuGet 稳定最新版，包括 `Avalonia` 12.0.2、`ReactiveUI.Avalonia` 12.0.1、`SharpCompress` 0.48.0、`YamlDotNet` 17.1.0、`Magick.NET-Q16-AnyCPU` 14.13.0、`MiniExcel` 1.43.1、`Microsoft.NET.Test.Sdk` 18.5.1、`xunit.v3` 3.2.2 等。
- 🔨[优化]-适配 `Avalonia` 12 / `ReactiveUI.Avalonia` 12 初始化 API，并继续避免引入 Avalonia 商业套件包。
- 🔨[优化]-迁移测试项目到 `xunit.v3`，移除 NuGet 已标记 Legacy 的 `xunit` v2 包。
- 🔨[优化]-将 `AppConfigHelper` 默认 App.config 读写改为基于 XML 的 appSettings 实现，避免 Native AOT 发布时依赖不安全的 `ConfigurationManager` 路径；保留兼容扩展方法并添加 trim 标注。
- 🔨[优化]-JSON 源生成重载、Demo JSON/XML 示例、JSON/YAML 字符串转换改为更适合 Native AOT 的实现；对象级 YAML API 保留并标注非 AOT 安全。
- 🔨[优化]-合并 `CodeWF.Tools.AvaloniaDemo.Desktop` 与 `CodeWF.Tools.AvaloniaDemo` 为单一 Avalonia 桌面测试工程，减少重复入口和发布配置。
- 😄[新增]-测试 Demo 接入 `CodeWF.LogViewer.Avalonia` 12.0.2.1，新增中文日志展示和文件记录，便于查看测试过程与结果。
- 🔨[优化]-重做 Demo 测试台界面，按配置、序列化、文件、网络、图片等分组覆盖常用帮助方法，并统一输出中文测试日志。
- 🔨[优化]-更新 `publish.bat` 一键发布脚本，指向合并后的 Demo 工程并使用中文发布输出。
- 🔨[优化]-为 DataAnnotations、TypeDescriptor、反射枚举等不适合 Native AOT 的 API 添加 trim/AOT 标注，`FormatBytes` 和部分枚举拆分逻辑改为 AOT 友好实现。
- 🔨[优化]-完善 nullable 安全、异步测试写法、配置读写、DataTable 导入、原生库方法缓存、压缩封装和图片资源释放。
- 🔨[优化]-新增开源规范文件：`.editorconfig`、`CONTRIBUTING.md`、`SECURITY.md`。
- 📝[说明]-Native AOT 发布仍会报告 `ReactiveUI.Avalonia` 12.0.1 和 `CodeWF.Log.Core` 12.0.2.1 包内部 trim/AOT 警告，仓库代码自身未再触发 AppConfig/XmlSerializer/反射 JSON/YAML 相关 AOT 警告。

## V1.3.0.0（2025-01-02）

- 🔨[优化]-CodeWF.Tools.Core: .NET原生功能扩展，无第三方库依赖
- 🔨[优化]-CodeWF.Tools.Files：文件操作类集合
- 🔨[优化]-CodeWF.Tools.Image：图片处理集合
- 🔨[优化]-CodeWF.Tools：集成前面的包，功能最全

## V1.2.8.0（2024-12-27）

- 😄[新增]-添加正则表达式特殊字符转换，提供一种简单的方式来构建正则表达式模式，使用户能够以更直观的符号（如*、%等）来表达他们想要的匹配模式。这降低了用户使用正则表达式的门槛，特别是在需要进行简单的文本搜索或过滤场景下，很有帮助。

## V1.2.7.0（2024-12-25）

- 😄[新增]-添加Icon图标转换
## 2026-06-08 仓库规范整理

- 统一文档维护入口：每个仓库只保留根目录 `README.md` 和根目录 `UpdateLog.md`，清理重复日志、英文文档和语言切换入口。
- 统一版本维护入口：包版本只在仓库根目录 `Directory.Build.props` 的 `<Version>` 节点维护，移除散落的程序集版本配置。
- 不再维护 `global.json`，SDK 选择交给本机或 CI 环境；NuGet 包和应用的目标框架在项目文件中明确声明。
- 统一 NuGet 包文档入口：包 README 统一引用仓库根 `README.md`，更新日志统一引用仓库根 `UpdateLog.md`。
