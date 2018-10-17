@echo off
rem -------------------------------------------------
rem AlarmWorkflow build script (RELEASE)
rem -------------------------------------------------
SET build="%~dp0msbuild.bat"
SET root=%~dp0\..

echo -------------------------------------------------
echo Restoring .nuget Packages

CALL RestorePackages.bat

echo -------------------------------------------------
echo Build Shared...
CALL %build% %root%\Shared\Shared.sln /p:Configuration=Release /verbosity:minimal

echo -------------------------------------------------
echo Build Windows-specific stuff...
CALL %build% %root%\Backend\Backend.sln /p:Configuration=Release /verbosity:minimal
CALL %build% %root%\BackendServices\BackendServices.sln /p:Configuration=Release /verbosity:minimal

CALL %build% %root%\AlarmSources\AlarmSources.sln /p:Configuration=Release /verbosity:minimal

CALL %build% %root%\Windows\Windows.sln /p:Configuration=Release /verbosity:minimal
CALL %build% %root%\WindowsUIWidgets\WindowsUIWidgets.sln /p:Configuration=Release /verbosity:minimal
CALL %build% %root%\Configuration\Configuration.sln /p:Configuration=Release /verbosity:minimal

CALL %build% %root%\Jobs\Engine\EngineJobs.sln /p:Configuration=Release /verbosity:minimal
CALL %build% %root%\Jobs\WindowsUI\WindowsUIJobs.sln /p:Configuration=Release /verbosity:minimal

CALL %build% %root%\Tools\Tools.sln /p:Configuration=Release /verbosity:minimal