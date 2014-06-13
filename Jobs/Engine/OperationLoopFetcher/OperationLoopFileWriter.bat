rem Schreibt Schleifeninfos der FMS-Programme in eine Datei, wo sie dann von AlarmWorkflow abgegriffen und
rem zu den Alarmen hinzugefügt werden.
rem
rem ----- HINWEIS -----
rem
rem Bevor diese Batch verwendet werden kann, müssen Sie einen gültigen Dateipfad
rem unter "{FILE_PATH}" eingeben.
rem
rem ----- HINWEIS -----

@echo off
echo %1;%date% %time% >> {FILE_PATH}