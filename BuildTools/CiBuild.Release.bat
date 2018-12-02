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
msbuild %root%\AlarmWorkflow.sln /p:Configuration=Release /logger:"C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"