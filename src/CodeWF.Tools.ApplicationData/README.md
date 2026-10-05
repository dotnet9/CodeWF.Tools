# CodeWF.Tools.ApplicationData

桌面应用数据目录与一次性迁移的标准实现（AOT 友好、零依赖）。

- 数据目录统一放 `%LOCALAPPDATA%\<应用名>`（Linux/macOS 随 .NET 标准取值）；
- 旧位置（`%APPDATA%\<应用名>`）有数据而新位置没有 → 一次性整目录迁移，旧目录保留作备份；
- 兼容错误迁移（内容少一级目录散落在 LOCALAPPDATA 根）的检测与自动修复；
- 便携模式：exe 旁存在标记文件时，数据目录跟随程序目录。

用法见 `AppDataMigrator.ResolveAndMigrate(...)`。
