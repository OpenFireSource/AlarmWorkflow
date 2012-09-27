@echo off
echo Build Shared...
msbuild Shared\AlarmWorkflow.Shared.sln /p:Configuration=Debug /verbosity:minimal

echo 
echo Build Windows-specific stuff...
msbuild Windows\AlarmWorkflow.Windows.sln /p:Configuration=Debug /verbosity:minimal

pause