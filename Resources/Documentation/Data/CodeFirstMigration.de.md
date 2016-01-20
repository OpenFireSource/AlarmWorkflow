# Migration der Datenbank auf Version v1.0

Dieses Dokument betrifft nur Nutzer, die AlarmWorkflow bereits vor der v1.0 im Einsatz hatten, und ihre Daten migrieren möchten.

Beginnend mit der Version v1.0 hat sich - Codeseitig - der Zugriff auf die Datenbank geändert.

Konkret bedeutet das, dass auf Entity Framework Code First gewechselt wurde, welches einiges an Erleichterung für künftige Datenbankaktualisierungen bringen wird, u.a. automatische Updates der Datenbankstruktur ohne Interaktion des Nutzers.

Für den Anwender bedeutet das, dass die bestehende Datenbank umgestellt werden muss.
Die nachfolgende Anleitung beschreibt die Vorgehensweise.

Es wird davon ausgegangen, dass der Leser Grundkenntnisse von **HeidiSQL**, **MySQL Workbench** oder **phpMyAdmin** besitzt.
Diese Anleitung basiert auf der Verwendung von **HeidiSQL** und **MySQL Workbench**, dürfte aber auch mit **phpMyAdmin** zu erledigen sein.

**Wichtiger Hinweis:** Bitte beachten Sie, dass für Datenverlust in keiner Weise Haftung übernommen wird! Im Zweifel wechseln Sie bitte zurück auf die vorherige Version und/oder fragen im Forum nach.

# Umstellung der Datenbank

## 1. Sicherung der bestehenden Datenbank

Es empfiehlt sich, dass die bestehende Datenbank umbenannt wird, um Datenverlust zu vermeiden.

Dies kann am einfachsten bewerkstelligt werden, wenn in der Datei **Backend.config** der Wert folgender Einstellung verändert wird:

  <add key="Server.DB.DatabaseName" value="alarmworkflow" />

Ändern Sie den Wert von **"alarmworkflow"** auf beispielsweise **"alarmworkflow_ng"** (für _n_ext _g_eneration, ein neutraler Name, kann aber beliebig sein).
So bleibt die bestehende Datenbank erhalten und die Anwendung wird dann die neue Datenbank anlegen.

## 2. Starten des Services

Die neue Datenbankstruktur wird automatisch durch den Code angelegt. Hierzu ist lediglich der Service zu starten.

**Hinweise:**
- Es empfiehlt sich, die Logdateien zu Öffnen, um eventuelle Probleme diagnostizieren zu können.
- Weiterhin kann dieser Prozess einige Zeit in Anspruch nehmen, aus Erfahrung zwischen einer und 60 Sekunden.

## 3. Übernahme der bestehenden Daten

Sehr wahrscheinlich sind bereits Daten vorhanden, die am besten übernommen werden sollten.

Dies gelingt sehr einfach mit **HeidiSQL** in folgenden Schritten:

1. Starten von **HeidiSQL** und Anmeldung am Server
2. Navigation zu "alarmworkflow", Rechtsklick, "Datenbank SQL Export"
3. Im Dialog "Tabellen Werkzeuge" folgende Einstellungen festlegen:
4. "Daten": "DELETE+ INSERT (truncate existing data)"
5. "Ausgabe": "Database"
6. "Datenbank": "alarmworkflow_ng"
7. Auf "Exportieren" klicken. Die Daten sollten dann in die neue Datenbank kopiert worden sein.

Alternativ kann es auch mit **MySQL Workbench** folgendermaßen bewerkstelligt werden:

1. Starten der Anwendung und Anmeldung am Server
2. Navigieren zu "alarmworkflow" -> "operation" (die bestehende Datenbank)
3. Kontextmenü aufrufen und "Select rows" wählen (im Deutschen ggf. anders)
4. Im sich öffnenden Dokument nun auf die Diskette neben "**Export/Import**" (1. Symbol) und die Datei speichern
5. Diesen Vorgang nun Wiederholen für die restlichen Tabellen
6. Nachdem alle vier Tabellen exportiert wurden, ist es erforderlich, dass in jeder exportierten Datei die *erste Zeile* gelöscht wird, da sonst der Importer nicht korrekt arbeiten kann!

In der neuen Datenbank müssen folgende Schritte für jede Tabelle ausführen:
1. Kontextmenü aufrufen und "Select rows" wählen (im Deutschen ggf. anders)
2. Im sich öffnenden Dokument nun auf den Ordner neben "**Export/Import**" (2. Symbol) und die exportierte Datei auswählen
3. Der Importvorgang kann ein paar Sekunden dauern. Nach Abschluss ist der aktiv gewordene Button "Apply" zu betätigen und das Fenster zu bestätigen.

**Hinweis:** Falls beim Export etwas nicht ordnungsgemäß funktioniert, haben Sie einerseits noch Ihr Backup, außerdem befinden sich Ihre Daten noch in der Ursprungsdatenbank.