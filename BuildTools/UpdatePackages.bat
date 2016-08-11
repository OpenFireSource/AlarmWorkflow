FOR /R "..\" %%G IN (*.sln) DO NuGet.exe update "%%G"
