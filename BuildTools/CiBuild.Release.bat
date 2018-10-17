@echo off
rem -------------------------------------------------
rem AlarmWorkflow build script (RELEASE)
rem -------------------------------------------------
SET root=%~dp0\..

echo -------------------------------------------------
echo Restoring .nuget Packages

CALL RestorePackages.bat

echo -------------------------------------------------
echo Build Shared...
msbuild %root%\Shared\Shared.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"

echo -------------------------------------------------
echo Build Windows-specific stuff...
msbuild %root%\Backend\Backend.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
msbuild %root%\BackendServices\BackendServices.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"

msbuild %root%\AlarmSources\AlarmSources.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"

msbuild %root%\Windows\Windows.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
msbuild %root%\WindowsUIWidgets\WindowsUIWidgets.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
msbuild %root%\Configuration\Configuration.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"

msbuild %root%\Jobs\Engine\EngineJobs.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
msbuild %root%\Jobs\WindowsUI\WindowsUIJobs.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"

msbuild %root%\Tools\Tools.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"