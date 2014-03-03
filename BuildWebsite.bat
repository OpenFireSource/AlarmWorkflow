@echo off
echo -------------------------------------------------
echo Build Website(s)...
msbuild Website\AlarmWorkflow.Website.Reports\AlarmWorkflow.Website.Reports.sln /p:Configuration=Debug /verbosity:minimal

pause