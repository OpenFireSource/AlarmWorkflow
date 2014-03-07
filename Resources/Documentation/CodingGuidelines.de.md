# Coding Guidelines

Damit der Code universell lesbar und besser wartbar ist, gibt es einige Regeln, an die sich jeder Entwickler halten sollte!
Dies hat nicht die Intention, zu diktieren, welcher Code geschrieben wird, sondern wie er geschrieben wird.
Es sind nicht viele Richtlininen, aber sie sollen befolgt werden, damit der Code auch in Jahren noch gut wartbar und lesbar ist.

## Zitat

Das Git-Projekt enthält ein Richtliniendokument, welches ein Zitat enthält, das eine gute Zusammenfassung darstellt:

https://github.com/git/git/blob/master/Documentation/CodingGuidelines

> Make your code readable and sensible, and don't try to be clever.
>
> As for more concrete guidelines, just imitate the existing code
> (this is a good guideline, no matter which project you are
> contributing to). It is always preferable to match the _local_
> convention. New code added to Git suite is expected to match
> the overall style of existing code. Modifications to existing
> code is expected to match the style the surrounding code already
> uses (even if it doesn't match the overall style of existing code).
>
> But if you must have a list of rules, here they are.

# Programmiersprachen

## Generell

* Programmier- und Kommentarsprache ist **Englisch**. Andere Sprachen sind zu vermeiden.
* Textdateien (inkl. Code) sind nach Möglichkeit im UTF-8-Format abzuspeichern, um Komplikationen vorzubeugen.
* Im Zweifel ist bereits existierender Code als Stilvorgabe zu betrachten.

## Code-Dokumentation

Dieser Abschnitt betrifft jegliche Code-Dokumentation, wie z. B. Kommentare, XML-Dokumentation oder javadoc.

* XML-Kommentare (Code-Dokumentation) sind als Hilfe zu verstehen.
* Keine Ein-Wort- oder leere Dokumentation.
* Keine Dokumentation, die das offensichtliche wiederholen ("don't state the obvious").
* Code-Kommentare (/* */ oder //) sind nach Möglichkeit zu vermeiden. Der Code sollte so geschrieben sein, dass keine Kommentare nötig sind.
* Übertreibt es nicht mit Kommentaren. Der Code sollte im Idealfall so geschrieben sein, dass der Code die Kommentare sind ("The Code is the Docu").
* Keine Kommentare sind manchmal besser als unnötige oder nichtssagende.

## Lizenzheader

Der Layout-Header ist in sämtlichen Codedateien einzubauen, unabhängig der gewählten Programmiersprache.

```
// This file is part of AlarmWorkflow.
//
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.
```

Simple Textdateien, HTML-Seiten oder JavaScript können, müssen aber nicht zwingend.

## C#

Diese Richtlinien betreffen **C#**.

### Benennungen

Typ|Beispiel
---|--------
Privates Feld|private bool _myField;
Private Konstante|private const bool MyConstantField;
Public Feld/Konstante|public bool MyPublicField;
Private Methode|private bool MyMethod(string input)
Public Methode|public bool MyMethod(string input, string format)
Lokale Parameter (in Methoden)|bool myField = true;
Klassen/Strukturen|class MyNewClass
Interfaces|interface IMyNewInterface

### Mustervorlage Dateilayout

Das nachfolgende Layout hat Mustergültigkeit für Klassen, Interfaces und kann als Vorlage verwendet werden.
Nicht benötigte *#regions* können gestrichen werden. Zusätzliche regions sind zu vermeiden!

```csharp
// This file is part of AlarmWorkflow.
//
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace AlarmWorkflow.*Namespace*
{
    /// <summary>
    /// A precise and concise description of what this type does.
    /// </summary>
    public class *MeinTyp*
    {
        #region Constants

        #endregion

        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeinTyp" /> class.
        /// </summary>
        public *MeinTyp*()
        {

        }

        #endregion

        #region Methods

        #endregion
    }
}
```

### Wichtiges

* **Ein** Typ (Enum/Klasse/Interface) **pro Datei**.
* Schlüsselwort *var* nur bei extrem langen Typennamen verwenden ( > 20 Zeichen).
* Objekte, die **IDisposable** implementieren, mit **using () { }** umrahmen, um Speicherlecks vorzubeugen.

```csharp
using (FileStream stream = File.OpenRead("C:\\MyFile.txt"))
{
}
```

* Keine Autorenbezeichnungen in Kommentaren/Namespaces/Ordnern verwenden. Dafür gibt es Versionskontrolle.
* Keine **#regions** innerhalb von anderen **#regions** oder Methoden.
* Möglichst keine **public nested types** verwenden (nur wenn absolut notwendig).
* Jeder **public** oder **protected** Member hat auch einen XML-Kommentar: **///<summary>** und ist sinnvoll zu beschreiben (s.o.).
* Visual Studio macht es leicht, den Code automatisch zu formatieren (Einrückungen einheitlich etc.). Dazu drückt im Editor: **STRG+E**, dann **D**. Dies sorgt für eine bessere Lesbarkeit.
* Primitive Datentypen (string, bool etc.) werden entsprechend der Aliase verwendet. Also *bool myBool = true* statt *Boolean myBool = true*.

### Wartbarkeit

* Methoden sollten im Idealfall nie länger als 50 Zeilen sein. Man muss viel Scrollen. Splittet dazu Methoden in Teilmethoden auf (**keine** Regions!).
* Zeilen sollten nicht länger als 140 Zeichen sein.
* Methodennamen sollten **sprechend** sein. Man soll im Idealfall sofort verstehen, was die Methode tut.
* Ebenso Variablennamen. Keine kryptischen Bezeichner verwenden.
* Lambda- oder LINQ-Ausdrücke nicht übertrieben oft benutzen. LINQ-Ausdrücke (`from where select`) sind aus Gründen der Übersichtlichkeit zu vermeiden.

## Java

Im Aufbau!

Bis auf Weiteres gelten die offiziellen Oracle-Java-Konventionen:
http://www.oracle.com/technetwork/java/javase/documentation/codeconvtoc-136057.html

## HTML/JavaScript

Bei HTML bzw. insbesondere JavaScript ist auf Lesbarkeit zu achten.