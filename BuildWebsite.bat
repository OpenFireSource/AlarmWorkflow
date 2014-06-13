@echo off
echo -------------------------------------------------
echo Build Website(s)...
msbuild Website\Website.sln /p:Configuration=Debug /verbosity:minimal

pause