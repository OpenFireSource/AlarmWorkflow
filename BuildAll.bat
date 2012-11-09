@echo off
echo Build Shared...
msbuild Shared\AlarmWorkflow.Shared.sln /p:Configuration=Debug /verbosity:minimal
msbuild Shared\AlarmWorkflow.Shared.Parser.sln /p:Configuration=Debug /verbosity:minimal
msbuild Shared\AlarmWorkflow.Shared.Jobs.sln /p:Configuration=Debug /verbosity:minimal

echo 
echo Build Windows-specific stuff...
msbuild Windows\AlarmWorkflow.Windows.sln /p:Configuration=Debug /verbosity:minimal
msbuild Windows\AlarmWorkflow.Windows.Configuration.sln /p:Configuration=Debug /verbosity:minimal

echo
echo Copy Files for the Setup
copy "Build" "Installer"

pause