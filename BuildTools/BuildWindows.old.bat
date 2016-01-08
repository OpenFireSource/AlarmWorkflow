@echo off
rem -------------------------------------------------
rem AlarmWorkflow build script (DEBUG)
rem 
rem Please change "Framework64" to "Framework" in the following path
rem if you're running this script on a x86 or 32-bit OS.
rem -------------------------------------------------
SET build=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe
SET root=%~dp0\..

echo -------------------------------------------------
echo Build Shared...
%build% %root%\Shared\Shared.sln /p:Configuration=Debug /verbosity:minimal

echo -------------------------------------------------
echo Build Windows-specific stuff...
%build% %root%\Backend\Backend.sln /p:Configuration=Debug /verbosity:minimal
%build% %root%\BackendServices\BackendServices.sln /p:Configuration=Debug /verbosity:minimal

%build% %root%\AlarmSources\AlarmSources.sln /p:Configuration=Debug /verbosity:minimal

%build% %root%\Windows\Windows.sln /p:Configuration=Debug /verbosity:minimal
%build% %root%\WindowsUIWidgets\WindowsUIWidgets.sln /p:Configuration=Debug /verbosity:minimal
%build% %root%\Configuration\Configuration.sln /p:Configuration=Debug /verbosity:minimal

%build% %root%\Jobs\Engine\EngineJobs.sln /p:Configuration=Debug /verbosity:minimal
%build% %root%\Jobs\WindowsUI\WindowsUIJobs.sln /p:Configuration=Debug /verbosity:minimal

%build% %root%\Tools\Tools.sln /p:Configuration=Debug /verbosity:minimal

pause