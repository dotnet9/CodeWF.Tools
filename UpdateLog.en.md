# Changelog

## V1.3.13.2 (2026-05-20)

- 🔨[优化]-Bumped the package version to `1.3.13.2`.
- 🔨[优化]-Updated the Avalonia demo package family to `Avalonia` 12.0.3.
- 🔨[优化]-Updated dependencies to the latest stable source-open NuGet versions checked for this repository: `coverlet.collector` 10.0.1, `MiniExcel` 1.44.1, `SharpCompress` 1.0.0, `System.Configuration.ConfigurationManager` 10.0.8, and `System.Text.Json` 10.0.8.
- 🔨[优化]-Enabled NuGet central transitive package pinning for stricter dependency resolution.
- 🔨[优化]-Adapted `SevenZipCompressor` to the SharpCompress 1.0 non-generic `IWritableArchive` and `ArchiveFactory.Create(ArchiveType.Zip)` APIs.

## V1.3.12.3 (2026-05-09)

- 😄[新增]-Added Central Package Management through `Directory.Packages.props`.
- 🔨[优化]-Updated dependencies to the latest stable free/open-source NuGet versions, including `Avalonia` 12.0.2, `ReactiveUI.Avalonia` 12.0.1, `SharpCompress` 0.48.0, `YamlDotNet` 17.1.0, `Magick.NET-Q16-AnyCPU` 14.13.0, `MiniExcel` 1.43.1, `Microsoft.NET.Test.Sdk` 18.5.1, and `xunit.v3` 3.2.2.
- 🔨[优化]-Adapted the Avalonia demo to `Avalonia` 12 / `ReactiveUI.Avalonia` 12 initialization APIs while continuing to avoid Avalonia commercial suite packages.
- 🔨[优化]-Migrated test projects to `xunit.v3` and removed the NuGet Legacy-marked `xunit` v2 package.
- 🔨[优化]-Changed `AppConfigHelper` default App.config access to XML-based appSettings read/write paths so Native AOT publishing no longer depends on the unsafe `ConfigurationManager` path; compatibility extension methods remain with trim annotations.
- 🔨[优化]-Added source-generated JSON overloads and updated demo JSON/XML plus JSON/YAML string conversion paths to be more Native AOT friendly; object-level YAML APIs are retained and annotated as not AOT-safe.
- 🔨[优化]-Merged `CodeWF.Tools.AvaloniaDemo.Desktop` and `CodeWF.Tools.AvaloniaDemo` into a single Avalonia desktop test project to remove duplicate startup and publish configuration.
- 😄[新增]-Added `CodeWF.LogViewer.Avalonia` 12.0.2.1 to the test demo for Simplified Chinese log display and file logging.
- 🔨[优化]-Redesigned the demo test bench and grouped coverage for configuration, serialization, file, network, image, and other helper APIs with Chinese test logs.
- 🔨[优化]-Updated the `publish.bat` one-click publish script to target the merged demo project and print Simplified Chinese publish output.
- 🔨[优化]-Added trim/AOT annotations for APIs based on DataAnnotations, TypeDescriptor, and reflection-based enum helpers; made `FormatBytes` and enum splitting paths more AOT friendly.
- 🔨[优化]-Improved nullable safety, async test usage, configuration helpers, DataTable import handling, native library method caching, compression wrappers, and image resource disposal.
- 🔨[优化]-Added open-source project files: `.editorconfig`, `CONTRIBUTING.md`, and `SECURITY.md`.
- 📝[说明]-Native AOT publishing still reports package-internal trim/AOT warnings from `ReactiveUI.Avalonia` 12.0.1 and `CodeWF.Log.Core` 12.0.2.1; repository code no longer emits AppConfig/XmlSerializer/reflection JSON/YAML AOT warnings.

## V1.3.0.0 (2025-01-02)

- 🔨[优化]-`CodeWF.Tools.Core`: .NET native extensions without third-party dependencies.
- 🔨[优化]-`CodeWF.Tools.Files`: file operation utilities.
- 🔨[优化]-`CodeWF.Tools.Image`: image processing utilities.
- 🔨[优化]-`CodeWF.Tools`: full package integrating the packages above.

## V1.2.8.0 (2024-12-27)

- 😄[新增]-Added regex special-character conversion to make simple search and filtering patterns easier to express with intuitive symbols such as `*` and `%`.

## V1.2.7.0 (2024-12-25)

- 😄[新增]-Added icon conversion.
