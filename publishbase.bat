@echo off
setlocal enabledelayedexpansion

if "%~1"=="" (
    set "project_paths=src\CodeWF.Tools.AvaloniaDemo"
) else (
    set "project_paths=%~1"
)

if "%~2"=="" (
    set "platforms=win-x64"
) else (
    set "platforms=%~2"
)

set "publish_root=%~dp0publish"
set "publish_failed=0"

for %%p in (%platforms%) do (
    set "rid=%%p"

    echo ========================================
    echo 正在发布 %%p...
    echo ========================================

    for %%d in (%project_paths%) do (
        for %%n in ("%%d") do set "project_name=%%~nxn"

        echo 正在发布 %%d 到 %%p...
        call :publish_with_profile "%%d" "!rid!" "!project_name!"
        if errorlevel 1 (
            echo 错误：%%d 发布到 %%p 失败
            set "publish_failed=1"
        )
    )
    echo.
)

if "%publish_failed%"=="1" (
    echo ========================================
    echo 发布失败，请检查上方错误。
    echo ========================================
    call :maybe_pause
    exit /b 1
)

echo ========================================
echo 所有平台发布成功。
echo ========================================
if /i not "%CODEX_NO_EXPLORER%"=="1" explorer "%publish_root%"
call :maybe_pause
goto :eof

:publish_with_profile
set "project_path=%~1"
set "rid=%~2"
set "project_name=%~3"
set "publish_profile=FolderProfile_%rid%"
set "profile_dir=%project_path%\Properties\PublishProfiles"

call :try_publish "%project_path%" "%publish_profile%" "%project_name%" "%profile_dir%" "%rid%"
exit /b %errorlevel%

:try_publish
set "project_path=%~1"
set "publish_profile=%~2"
set "project_name=%~3"
set "profile_dir=%~4"
set "rid=%~5"
set "profile_file=%profile_dir%\%publish_profile%.pubxml"
set "target_framework="
set "runtime_identifier="
set "profile_metadata="

if not exist "%profile_file%" (
    echo   - 未找到发布配置 %publish_profile%，改用项目默认发布参数。
    call :publish_direct "%project_path%" "%rid%" "%project_name%"
    exit /b %errorlevel%
)

for /f "usebackq delims=" %%m in (`powershell -NoProfile -Command "$xml = [xml](Get-Content -LiteralPath '%profile_file%' -Raw -Encoding UTF8); '{0}|{1}' -f $xml.Project.PropertyGroup.TargetFramework, $xml.Project.PropertyGroup.RuntimeIdentifier"`) do (
    set "profile_metadata=%%m"
)

for /f "tokens=1,2 delims=|" %%f in ("%profile_metadata%") do (
    set "target_framework=%%f"
    set "runtime_identifier=%%g"
)

if not defined target_framework (
    echo 发布配置缺少目标框架：%profile_file%
    exit /b 1
)

if not defined runtime_identifier (
    echo 发布配置缺少运行时标识：%profile_file%
    exit /b 1
)

echo   - 使用发布配置 %publish_profile%...
dotnet publish "%project_path%" -f %target_framework% -r %runtime_identifier% -p:PublishProfile=%publish_profile%
if errorlevel 1 exit /b 1

if exist "%publish_root%" (
    set /a removed_pdb_count=0
    for /r "%publish_root%" %%f in (*.pdb) do (
        del /q "%%f" 2>nul
        if not exist "%%f" set /a removed_pdb_count+=1
    )
    if !removed_pdb_count! gtr 0 echo   - 已清理 !removed_pdb_count! 个 *.pdb 文件
)

echo   - 发布成功：%project_name% / %rid%
exit /b 0

:publish_direct
set "project_path=%~1"
set "rid=%~2"
set "project_name=%~3"
set "target_framework=net10.0-windows"
set "publish_dir=%publish_root%\%rid%\%project_name%"

if not defined target_framework (
    echo 无法从项目文件读取目标框架：%project_path%
    exit /b 1
)

dotnet publish "%project_path%" -c Release -f %target_framework% -r %rid% --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishAot=true -o "%publish_dir%"
if errorlevel 1 exit /b 1

if exist "%publish_dir%" (
    set /a removed_pdb_count=0
    for /r "%publish_dir%" %%f in (*.pdb) do (
        del /q "%%f" 2>nul
        if not exist "%%f" set /a removed_pdb_count+=1
    )
    if !removed_pdb_count! gtr 0 echo   - 已清理 !removed_pdb_count! 个 *.pdb 文件
)

echo   - 发布成功：%project_name% / %rid%
exit /b 0

:maybe_pause
if /i not "%CODEX_NO_PAUSE%"=="1" pause
exit /b 0
