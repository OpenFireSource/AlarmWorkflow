Alarmworkflow steht unter GPL v3. Näheres dazu in der gpl-3.0.txt

Standardmäßig wird die OCR Software cuneiform mitgeliefert, die nach unserer Erkenntnis bessere Erkenungsraten liefert. Tesseract kann weiter genutzt werden, muss aber http://code.google.com/p/tesseract-ocr/ bezogen werden.

Es gibt zur Zeit nur Unterstützung für die Faxe der Leitstelle München Land. Ich habe zwar schon ein paar Testfaxe von anderen Leitstellen erhalten, dafür wird es aber erst in kommenden Versionen unterstützung geben.

Das ganze ist ein Windows Dienst also wenn ihr den Installer installiert gibt es keine Exe zum Doppel klicken :-) Start->Systemsteuerung -> Verwaltung -> Dienste -> Alarmworkflow Service
Dort könnt ihr den Dienst starten stoppen.

Bevor ihr dies aber tut solltet ihr Alarmworkflow noch richtig konfigurieren. In dem Verzeichnis in welches ihr Alarmworkflow installiert habt, liegt eine Datei AlarmWorkflow.xml. In dieser Datei findet ihr alle Möglichkeiten um das Projekt auf eure Bedürfnisse an zu passen.

Diese Config ist in 5 Bereiche gegliedert:

1. replacing: Hier könnt ihr Wörter durch andere ersetzen lassen, dies macht Sinn wenn die OCR Software ständig ein Wort falsch erkennt. Unter tesseract wurde bei unserem fax immer statt Straße StraBe erkannt.

2. Jobs: Alarmworkflow ist in mehrere Module unterteilt die so genannten Jobs. (z.B. Email verschiken, SMS verschiken, Datenbank eintrag erstellen etc.) Jeder Job hat eigene Einstellungsmöglichkeiten diese findet ihr hier.

    * Database: Eine MySQL Datenbank in die alle Details geschrieben werden. Es wird in die Tabelle tb_einsatz geschrieben. Es müssen die folgenden Spalten vorhanden sein: Einsatznr, Einsatzort, Einsatzplan, Hinweis, Kreuzung, Meldebild, Mitteiler, Objekt, Ort, Strasse, Stichwort. Jeweils ein VARCHAR MAX oder so. Da schau ich aber noch mal nach oder stelle ein kleines SQL-Script zur Verfügung wenn es damit zu Problemen kommt.
    * Mailing: Hier wird ein SMTP Server konfiguriert und Email-Adressen eingetragen. Nur eine Adresse die mit debug=”true” gekennzeichnet ist, bekommt auch eine Email wenn Alarmworkflow im Debug Modus gestartet ist. Siehe Punkt 5.
    * SMS: Hier werden die Zugangsdaten zu SMS77 hinterlegt und die Handynummern. Nur eine Nummer die mit debug=”true” gekennzeichnet ist, bekommt auch eine SMS wenn Alarmworkflow im Debug Modus gestartet ist. Siehe Punkt 5.
    * DisplayWakeUp: Hier wird das TFTPowerControl konfiguriert, das bei uns die Monitore bei einem Einsatz anschaltet.

3. Logging: Alarmworkflow kann Fehler, Information und Warnings Loggen. Dies kann man hier konfigurieren. Zur Zeit existieren 2 Möglichkeiten: Kein Logging oder das Loggen in die Windows Event Anzeige.

4. Service: In diesem Bereich konfiguriert ihr Alarmworkflow selbst.

    * FaxPath ist das Verzeichnis in dem euer Faxserver die tiff Bilder ablegt.
    * ArchievPath ist das Verzeichnis in welches die tiff Bilder nach dem Auswerten kopiert werden.
    * AnalysisPath ist ein Ordner in den die Auswertungen der OCRSoftware gespeichert werden.
    * OCRSoftware spezifiziert die Software die für die OCR Auswertung der Bilder benutz werden soll es gibt 2 Möglichkeiten: cuniform (Standard), oder tesseract. Path gibt an in welchem Verzeichnis nach der OCR Software gesucht werden soll. Wenn kein Pfad angegeben ist dann wird das Installationverzeichnis genommen.
    * AlarmfaxParser gibt an welcher Parser zur Auswertung des OCR Ergebnisses verwendet werden soll. Zur Zeit gibt des nur den MucLandParser, weitere folgen.

5. AktiveJobs: Hier kann jeder einzelne Job an- und abgeschalten werden. Wenn DebugMode auf true steht, startet Alarmworkflow im Debug Modus. In diesem Modus werden SMSen und Emails nur an die Leute geschickt die in der Job Konfiguration mit debug=”true” gekennzeichnet sind.

Hinweise:

Nach einer Änderung in der AlarmWorkflow.xml muss der Dienst neu gestartet werden.
Es sind noch nicht alle Fehler richtig abgefangen, wenn z.B. ein Verzeichnis bei Punkt 4 Service nicht existiert wird der Dienst mit nicht Aussagekräftigem Fehler beendet.

Grüße
Andreas