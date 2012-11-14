@echo off
echo -------------------------------------------------
echo Build Website(s)...
msbuild Website\AlarmWorkflow.Website.Asp\AlarmWorkflow.Website.Asp.sln /p:Configuration=Debug /verbosity:minimal

pause