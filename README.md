# CodeWF.Tools

[简体中文](README-zh_CN.md) | English

| NuGet Name | NuGet url | Download |
|------|-----------|--------|
| CodeWF.Tools | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools)](https://www.nuget.org/packages/CodeWF.Tools/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools)](https://www.nuget.org/packages/CodeWF.Tools/) |
| CodeWF.Tools.Core | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools.Core.svg)](https://www.nuget.org/packages/CodeWF.Tools.Core/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools.Core.svg)](https://www.nuget.org/packages/CodeWF.Tools.Core/) |
| CodeWF.Tools.Files | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools.Files.svg)](https://www.nuget.org/packages/CodeWF.Tools.Files/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools.Files.svg)](https://www.nuget.org/packages/CodeWF.Tools.Files/) |
| CodeWF.Tools.Image | [![NuGet](https://img.shields.io/nuget/v/CodeWF.Tools.Image.svg)](https://www.nuget.org/packages/CodeWF.Tools.Image/) | [![NuGet](https://img.shields.io/nuget/dt/CodeWF.Tools.Image.svg)](https://www.nuget.org/packages/CodeWF.Tools.Image/) |

**CodeWF.Tools, making C# coding simpler.**

CodeWF.Tools is an open-source utility library for C# developers. It provides practical helpers for strings, dates, files, JSON/YAML conversion, image utilities, and common development workflows.

## Packages

- `CodeWF.Tools.Core`: native .NET extensions without third-party dependencies.
- `CodeWF.Tools.Files`: file, export, JSON, and YAML utilities.
- `CodeWF.Tools.Image`: image and QR code utilities.
- `CodeWF.Tools`: the combined package.

## Development

This repository uses NuGet Central Package Management. Package versions are maintained in `Directory.Packages.props`.

- Changelog: [中文](UpdateLog.md) / [English](UpdateLog.en.md)
- Contributing: [CONTRIBUTING.md](CONTRIBUTING.md)
- Security: [SECURITY.md](SECURITY.md)

## Thanks

- Masuit.Tools：https://github.com/ldqk/Masuit.Tools

## Third-Party Open Source Audit

Checked on 2026-05-20 with NuGet metadata, restored `project.assets.json`, and upstream source/license links. MIT / Apache-2.0 / BSD are preferred.

Remediation:

- Updated `Magick.NET-Q16-AnyCPU` from `14.13.0` to `14.13.1`; `dotnet build CodeWF.Tools.slnx -c Release -m:1` then completed with 0 warnings and 0 errors.
- Updated the Avalonia demo package family from `12.0.2` to `12.0.3`.
- Updated `MiniExcel` from `1.43.1` to `1.44.1`, `SharpCompress` from `0.48.0` to `1.0.0`, `System.Configuration.ConfigurationManager` / `System.Text.Json` from `10.0.7` to `10.0.8`, and `coverlet.collector` from `10.0.0` to `10.0.1`.
- Enabled NuGet central transitive pinning and adapted `SevenZipCompressor` to the SharpCompress 1.0 non-generic `IWritableArchive` / `ArchiveFactory.Create(ArchiveType.Zip)` APIs.
- Bumped the package version to `1.3.13.2` for the dependency update.
- Verification completed with `dotnet build CodeWF.Tools.slnx -c Debug --no-restore`, `dotnet test CodeWF.Tools.slnx -c Debug --no-build`, and `dotnet build CodeWF.Tools.slnx -c Release --no-restore -m:1`.

| Package | Usage | License | Source | Status |
| --- | --- | --- | --- | --- |
| `CsvHelper` | `CodeWF.Tools.Files` CSV helpers | MS-PL OR Apache-2.0 | https://github.com/JoshClose/CsvHelper | Approved via Apache-2.0 option |
| `Magick.NET-Q16-AnyCPU` | `CodeWF.Tools.Image` image helpers | Apache-2.0 | https://github.com/dlemstra/Magick.NET | Approved, `14.13.1` |
| `MiniExcel` | `CodeWF.Tools.Files` Excel helpers | Apache-2.0 | https://github.com/mini-software/MiniExcel | Approved, `1.44.1` |
| `SharpCompress` | `CodeWF.Tools.Files` archive helpers | MIT | https://github.com/adamhathcock/sharpcompress | Approved, `1.0.0` |
| `System.Configuration.ConfigurationManager` / `System.Text.Json` | Configuration and JSON helpers | MIT | https://github.com/dotnet/dotnet | Approved, `10.0.8` |
| `YamlDotNet` | `CodeWF.Tools.Files` YAML helpers | MIT | https://github.com/aaubry/YamlDotNet | Approved |
| `ZXing.Net.Bindings.Magick` | QR/barcode helpers | Apache-2.0 | https://github.com/micjahn/ZXing.Net | Approved |
| `Avalonia` / `Avalonia.Desktop` / `Avalonia.Fonts.Inter` / `Avalonia.Themes.Fluent` | Demo only | MIT | https://github.com/AvaloniaUI/Avalonia | Approved, `12.0.3` |
| `Bogus` | Test data only | MIT | https://github.com/bchavez/Bogus | Approved |
| `CodeWF.LogViewer.Avalonia` | Demo only | MIT | https://github.com/dotnet9/CodeWF.LogViewer | Own open-source package |
| `ReactiveUI.Avalonia` | Demo only | MIT | https://github.com/reactiveui/reactiveui | Approved |
| `VC-LTL` | Windows demo runtime compatibility | EPL-2.0 | https://github.com/Chuyu-Team/VC-LTL5 | Source-open; approved under the source-traceable non-preferred license rule |
| `Microsoft.NET.Test.Sdk` / `coverlet.collector` | Tests | MIT | https://github.com/microsoft/vstest / https://github.com/coverlet-coverage/coverlet | Approved, `Microsoft.NET.Test.Sdk` `18.5.1`, `coverlet.collector` `10.0.1` |
| `xunit.v3` / `xunit.runner.visualstudio` | Tests | Apache-2.0 | https://github.com/xunit/xunit | Approved |

Transitive dependency check:

| Dependency group | Representative packages | License | Source | Status |
| --- | --- | --- | --- | --- |
| .NET runtime/configuration stack | `System.Diagnostics.EventLog`, `System.IO.Pipelines`, `System.Text.Encodings.Web`, `System.Security.Cryptography.ProtectedData` | MIT | https://github.com/dotnet/dotnet | Source-open |
| Magick.NET core | `Magick.NET.Core` | Apache-2.0 | https://github.com/dlemstra/Magick.NET | Source-open |
| ZXing core | `ZXing.Net` | Apache-2.0 | https://github.com/micjahn/ZXing.Net | Source-open |
| Avalonia stack | `Avalonia.*`, `Avalonia.BuildServices` | MIT | https://github.com/AvaloniaUI/Avalonia | Source-open |
| ANGLE native package | `Avalonia.Angle.Windows.Natives` | BSD-style license file | https://github.com/AvaloniaUI/angle | Source-open |
| Skia/HarfBuzz bindings and native assets | `SkiaSharp*`, `HarfBuzzSharp*` | MIT package license; native projects are source-open | https://github.com/mono/SkiaSharp | Source-open |
| Reactive stack | `ReactiveUI`, `DynamicData`, `Splat`, `System.Reactive` | MIT | https://github.com/reactiveui/reactiveui / https://github.com/reactiveui/DynamicData / https://github.com/reactiveui/splat / https://github.com/dotnet/reactive | Source-open |
| Linux desktop interop | `MicroCom.Runtime`, `Tmds.DBus.Protocol` | MIT | https://github.com/kekekeks/MicroCom / https://github.com/tmds/Tmds.DBus | Source-open |
| Own logging dependency | `CodeWF.Log.Core` | MIT | https://github.com/dotnet9/CodeWF.LogViewer | Own source-open package |
| Test toolchain | `Microsoft.*`, `Newtonsoft.Json`, `xunit.*` | MIT / Apache-2.0 | https://github.com/microsoft/vstest / https://github.com/microsoft/testfx / https://github.com/JamesNK/Newtonsoft.Json / https://github.com/xunit/xunit | Source-open |

No non-open or black-box dependency is intentionally retained.
