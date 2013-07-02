rem Schreibt Schleifeninfos der FMS-Programme in eine Datei,
rem wo sie dann von AlarmWorkflow abgegriffen und zu den Alarmen
rem hinzugefügt werden.
rem

rem ----- TODO! -----
rem
rem Bevor diese Batch verwendet werden kann, müssen Sie einen gültigen Dateipfad 
rem unter "{FILE_PATH}" eingeben. Der SYSTEM-Account muss Schreibrechte in diesem Pfad haben!
rem
rem Für weitere Informationen sehen Sie bitte im Wiki nach unter:
rem TODOTODOTODOTODOTODOTODOTODOTODO
rem
rem ----- TODO! -----

@echo off
echo %1;%date% %time% >> {FILE_PATH}