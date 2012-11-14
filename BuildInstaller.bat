@echo off
call "BuildWindows.bat"

echo -------------------------------------------------
echo Copy Files for the Setup ...
xcopy "Build" "Installer\Build\" /Y /S

pause