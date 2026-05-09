# Contributing / 贡献指南

Thanks for helping improve CodeWF.Tools.

感谢你帮助改进 CodeWF.Tools。

## Development

- Use the .NET SDK versions required by the target frameworks in the solution.
- Run `dotnet restore CodeWF.Tools.slnx --force-evaluate` after changing package versions.
- Run `dotnet build CodeWF.Tools.slnx --no-restore` before submitting changes.
- Keep package versions in `Directory.Packages.props`; project files should not carry inline package versions.
- Use free/open-source NuGet packages only. Avoid commercial packages or packages that require paid runtime behavior.

## Pull Requests

- Keep changes focused and explain the reason for behavior changes.
- Add or update tests when changing public behavior.
- Update both `UpdateLog.md` and `UpdateLog.en.md` for user-facing changes.
- Do not commit generated build output from `Output/`, `.vs/`, or IDE caches.

## 开发要求

- 按解决方案目标框架使用对应 .NET SDK。
- 修改包版本后运行 `dotnet restore CodeWF.Tools.slnx --force-evaluate`。
- 提交前运行 `dotnet build CodeWF.Tools.slnx --no-restore`。
- NuGet 版本统一维护在 `Directory.Packages.props`，不要在项目文件中写内联版本。
- 只使用免费/开源 NuGet 包，避免商业包或需要付费运行能力的包。

## PR 要求

- 保持改动聚焦，并说明行为变化原因。
- 修改公开行为时补充或更新测试。
- 面向用户的变更需要同步更新 `UpdateLog.md` 和 `UpdateLog.en.md`。
- 不提交 `Output/`、`.vs/` 或 IDE 缓存等生成文件。
