@echo off
echo -------------------------------------------------
echo Build submodules...
msbuild externals\S22.Imap\S22.Imap.sln /p:Configuration=Release /verbosity:minimal

echo -------------------------------------------------
echo Build Shared...
msbuild Shared\Shared.sln /p:Configuration=Debug /verbosity:minimal

echo -------------------------------------------------
echo Build Windows-specific stuff...
msbuild Backend\Backend.sln /p:Configuration=Debug /verbosity:minimal
msbuild BackendServices\BackendServices.sln /p:Configuration=Debug /verbosity:minimal

msbuild AlarmSources\AlarmSources.sln /p:Configuration=Debug /verbosity:minimal
msbuild Parsers\Parsers.sln /p:Configuration=Debug /verbosity:minimal

msbuild Windows\Windows.sln /p:Configuration=Debug /verbosity:minimal
msbuild WindowsUIWidgets\WindowsUIWidgets.sln /p:Configuration=Debug /verbosity:minimal
msbuild Configuration\Configuration.sln /p:Configuration=Debug /verbosity:minimal

msbuild Jobs\Engine\EngineJobs.sln /p:Configuration=Debug /verbosity:minimal
msbuild Jobs\WindowsUI\WindowsUIJobs.sln /p:Configuration=Debug /verbosity:minimal

pause