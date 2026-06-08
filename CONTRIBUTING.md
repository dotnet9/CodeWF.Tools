# 贡献指南

感谢你帮助改进 CodeWF.Tools。

## 开发要求

- 按解决方案目标框架使用对应 .NET SDK。
- 修改包版本后运行 `dotnet restore CodeWF.Tools.slnx --force-evaluate`。
- 提交前运行 `dotnet build CodeWF.Tools.slnx --no-restore`。
- NuGet 版本统一维护在 `Directory.Packages.props`，不要在项目文件中写内联版本。
- 只使用免费/开源 NuGet 包，避免商业包或需要付费运行能力的包。

## PR 要求

- 保持改动聚焦，并说明行为变化原因。
- 修改公开行为时补充或更新测试。
- 面向用户的变更需要同步更新根目录 `UpdateLog.md`。
- 不提交 `Output/`、`.vs/` 或 IDE 缓存等生成文件。
