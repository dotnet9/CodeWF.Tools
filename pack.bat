@echo off
setlocal

pushd "%~dp0"

set "CONFIGURATION=Release"
set "ARTIFACTS_DIR=%CD%\artifacts"
set "PACKAGES_DIR=%ARTIFACTS_DIR%\packages"

if exist "%PACKAGES_DIR%" rmdir /s /q "%PACKAGES_DIR%"
mkdir "%PACKAGES_DIR%"

for %%P in (
    "src\CodeWF.Tools.Core\CodeWF.Tools.Core.csproj"
    "src\CodeWF.Tools.Files\CodeWF.Tools.Files.csproj"
    "src\CodeWF.Tools.Image\CodeWF.Tools.Image.csproj"
    "src\CodeWF.Tools\CodeWF.Tools.csproj"
) do (
    echo Restoring %%~P...
    dotnet restore %%~P
    if errorlevel 1 goto :error
    echo Building %%~P...
    dotnet build %%~P -c %CONFIGURATION% --no-restore /p:GeneratePackageOnBuild=false
    if errorlevel 1 goto :error
    echo Packing %%~P...
    dotnet pack %%~P -c %CONFIGURATION% --no-build --no-restore -o "%PACKAGES_DIR%"
    if errorlevel 1 goto :error
)

for /r "%PACKAGES_DIR%" %%F in (*.pdb) do del /q "%%F" 2>nul

echo.
echo Packages are available in:
echo %PACKAGES_DIR%

popd
exit /b 0

:error
set "EXIT_CODE=%ERRORLEVEL%"
echo.
echo Pack failed with exit code %EXIT_CODE%.
popd
exit /b %EXIT_CODE%
