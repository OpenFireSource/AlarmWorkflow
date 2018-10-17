@echo off
set VSwhere="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"

for /f "usebackq tokens=*" %%i in (`%VSwhere% -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`) do (
  set InstallDir=%%i
)

SET MSBuild="%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe"
if exist %MSBuild% (
  %MSBuild% %*
) else (
  echo %MSBuild% not found!
)