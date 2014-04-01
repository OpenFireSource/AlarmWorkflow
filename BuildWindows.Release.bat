@echo off
echo -------------------------------------------------
echo Build Shared...
msbuild Shared\AlarmWorkflow.Shared.sln /p:Configuration=Release /verbosity:minimal

echo -------------------------------------------------
echo Build Windows-specific stuff...
msbuild Backend\AlarmWorkflow.Backend.sln /p:Configuration=Release /verbosity:minimal
msbuild BackendServices\AlarmWorkflow.BackendServices.sln /p:Configuration=Release /verbosity:minimal

msbuild Shared\AlarmWorkflow.Shared.AlarmSources.sln /p:Configuration=Release /verbosity:minimal
msbuild Shared\AlarmWorkflow.Shared.Parser.sln /p:Configuration=Release /verbosity:minimal
msbuild Shared\AlarmWorkflow.Shared.Jobs.sln /p:Configuration=Release /verbosity:minimal

msbuild Windows\AlarmWorkflow.Windows.sln /p:Configuration=Release /verbosity:minimal
msbuild Windows\AlarmWorkflow.Windows.Configuration.sln /p:Configuration=Release /verbosity:minimal
msbuild Windows\AlarmWorkflow.Windows.UIJobs.sln /p:Configuration=Release /verbosity:minimal
msbuild Windows\AlarmWorkflow.Windows.OperationViewer.sln /p:Configuration=Release /verbosity:minimal

msbuild Windows\AlarmWorkflow.Windows.UIWidget.sln /p:Configuration=Release /verbosity:minimal

pause