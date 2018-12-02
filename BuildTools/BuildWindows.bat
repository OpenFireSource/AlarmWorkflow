@echo off
rem -------------------------------------------------
rem AlarmWorkflow build script (DEBUG)
rem -------------------------------------------------
SET build="%~dp0msbuild.bat"
SET root=%~dp0\..

echo -------------------------------------------------
echo Restoring .nuget Packages

CALL RestorePackages.bat

echo -------------------------------------------------
echo Build AlarmWorkflow ...
CALL %build% %root%\AlarmWorkflow.sln /p:Configuration=Debug /verbosity:minimal