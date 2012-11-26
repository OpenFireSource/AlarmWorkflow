@echo off
xcopy "Build\Config\*.xml" "InstallerTemp\Config\*.xml" /Y
xcopy "Build\*.sdf" "InstallerTemp\*.sdf" /Y
xcopy "Build\*.dll" "InstallerTemp\*.dll" /Y
xcopy "Build\*.exe" "InstallerTemp\*.exe" /Y
xcopy "Build\*.manifest" "InstallerTemp\*.manifest" /Y
xcopy "Build\*.bat" "InstallerTemp\*.bat" /Y
xcopy "Build\*.config" "InstallerTemp\*.config" /Y

rem *** Special treatments (until better solution is found) ***
rem Maybe move that into Config-dir?
xcopy "Build\Controlfile.xml" "InstallerTemp\*" /Y
xcopy "Shared\AlarmWorkflow.Job.SQLCEDatabaseJob\SQLCEDatabase.sdf" "InstallerTemp\*" /Y

rem Delete unwanted files
del "InstallerTemp\*vshost*"
del "InstallerTemp\*AutoUpdater*"

pause