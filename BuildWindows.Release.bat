@echo off
rem -------------------------------------------------
rem AlarmWorkflow build script (RELEASE)
rem 
rem Please change "Framework64" to "Framework" in the following path
rem if you're running this script on a x86 or 32-bit OS.
rem -------------------------------------------------
SET build=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\msbuild.exe

echo -------------------------------------------------
echo Build Shared...
%build% Shared\Shared.sln /p:Configuration=Release /verbosity:minimal

echo -------------------------------------------------
echo Build Windows-specific stuff...
%build% Backend\Backend.sln /p:Configuration=Release /verbosity:minimal
%build% BackendServices\BackendServices.sln /p:Configuration=Release /verbosity:minimal

%build% AlarmSources\AlarmSources.sln /p:Configuration=Release /verbosity:minimal

%build% Windows\Windows.sln /p:Configuration=Release /verbosity:minimal
%build% WindowsUIWidgets\WindowsUIWidgets.sln /p:Configuration=Release /verbosity:minimal
%build% Configuration\Configuration.sln /p:Configuration=Release /verbosity:minimal

%build% Jobs\Engine\EngineJobs.sln /p:Configuration=Release /verbosity:minimal
%build% Jobs\WindowsUI\WindowsUIJobs.sln /p:Configuration=Release /verbosity:minimal

%build% Tools\Tools.sln /p:Configuration=Release /verbosity:minimal

pause