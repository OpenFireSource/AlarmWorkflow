@echo off
rem -------------------------------------------------
rem AlarmWorkflow build script (RELEASE)
rem -------------------------------------------------
SET build="C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"

echo -------------------------------------------------
echo Restoring .nuget Packages

.nuget\NuGet.exe restore Shared\Shared.sln
.nuget\NuGet.exe restore Backend\Backend.sln
.nuget\NuGet.exe restore AlarmSources\AlarmSources.sln

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