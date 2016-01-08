Dieses Dokument soll als Anlaufstelle für die Entwicklung von **AlarmWorkflow** dienen.

# Allgemeines

## Entwicklungsumgebung

Eine Entwicklungsumgebung ist notwendig, damit Sie den Quellcode von AlarmWorkflow kompilieren können. Im Projekt wird **Microsoft Visual Studio 2013** verwendet.
Hiervon gibt es eine sog. **Community Edition**, welche alle Features der Vollversion bietet und für Open Source-Projekte generell kostenlos ist 

Sie können dieses unter https://www.visualstudio.com/products/visual-studio-community-vs beziehen.

Das von AlarmWorkflow benötigte **Microsoft .Net Framework 4.6** wird automatisch mitinstalliert, falls es noch nicht installiert ist.

**Hinweis**: Während des Buildvorgangs werden Komponenten von Dritten heruntergeladen (z. B. log4net). Stellen Sie sicher, dass zumindest beim ersten Buildvorgang eine Internetverbindung besteht!

## Sourcecode

Es wird empfohlen, die von den Entwicklern zu Verfügung gestellte Version zu verwenden. Sollten Sie jedoch eigenständig Modifikationen vornehmen so besteht die Möglichkeit über GitHub die Sourcecodes zu beziehen.

## Programmierstil

Die Programmierrichtlinien befinden sich in der Datei *CodingGuidelines.de.md*.

## Mitentwickeln/Beitragen

Es ist erwünscht, dass Programmierkundige zu AlarmWorkflow beitragen! Hierfür geht ihr bitte den unter Git üblichen Weg der **Pull requests**.
Informationen dazu gibt es unter: https://help.github.com/articles/using-pull-requests

# Anforderungen an Patches und Pull Requests

## Coding Guidelines
Ein Code Style ist zwingend erforderlich, damit der Code immer einheitlich und leserlich bleibt. Ein guter, einheitlicher Code Style hilft allen Beteiligten und vor allem auch Neulingen, sich schnell zurechtzufinden, da man immer davon ausgehen kann, dass der Code stets uniform aussieht.

Hierzu sind die Coding Guidelines zu beachten und von Zeit zu Zeit zu besuchen, falls es zu Änderungen kam.
Siehe dazu die Datei *CodingGuidelines.de.md*.

## Contribution Guidelines
Es werden gewisse Standards vorgegeben, an die sich Beiträge zu halten haben, damit die Codebasis eine konsistente Qualität behalten kann.

## Codequalität
Zwingende Voraussetzung ist die Einhaltung der Coding Guidelines (s.o.). Beiträge, welche diese nicht erfüllen, werden abgelehnt und müssen nachgebessert werden.
Dies hat den nüchternen Hintergrund der Wartbarkeit. Einheitlicher Code ist besser wartbar, kaschiert im besten Falle keine Fehler und ist leichter zu verstehen.

## Source branch
Als source branch, also der Zweig, auf dem eure Commits basieren, soll der Zweig "next" dienen.
Dies dient dazu, den Hauptzweig von Änderungen jeglicher Natur freizuhalten, damit auf dem Hauptzweig stets ein stabiler und funktionierender Stand gegeben ist.
Dies soll allerdings **nicht** bedeuten, dass **next** als "Spielwiese" zu betrachten ist! ;-)

## Commit-Umfang
Es ist immer vorzuziehen, einen Commit als "atomare Einheit" zu handhaben. Das bedeutet:
Keine Vermischung von zwei oder mehr Themengebieten, die unabhängig voneinander sind. Dies ist im Regelfall möglich, kann aber auch seltene Ausnahmen geben. Diese sind dann idealerweise im Commit-Kommentar zu begründen.

## Commit-Nachrichten
Ein Commit ist grundsätzlich analog zu folgendem Beispiel aufzubauen:

> Kurze, aber präzise Beschreibung
>
> (Absatz)
>
> Längere Ausführung, was der Commit macht. Eine Beschreibung ist besonders bei größeren Commits zu verfassen!

## PR-Commits
Es ist Usus und erleichtert es den Entwicklern, wenn Beitragende einen PR erst dann pushen, wenn alle enthaltenen Commits auch wirklich sinnvoll sind. Dies ist z.B. bei folgendem PR **nicht** der Fall:

> 5: Habe dies hinzugefügt
> 4: Habe einen Buchstaben geändert
> 3: Rollback von #2
> 2: Einen String hinzugefügt
> 1: [Shared] Neue Methode

PRs dieser Art führen nur zum unnötigen aufblasen des Repositories und haben keinerlei Informationsgehalt; zumal manche Commits/Messages gegen oben genannte Regeln verstoßen.

Dies könnte z.B. locker in einem einzigen Patch überreicht werden, oder alternativ eben ein PR auf einem speziellen Branch mit nur einem Commit.

Grundsätzlich sind Commits nach folgender, einfachen Eselsbrücke zu erstellen:

> So detailliert wie nötig, so atomar (in sich geschlossen) wie möglich.

## Aktualisierung der Dokumentation/Release Notes
Bei Änderungen, welche Features hinzufügen, entfernen, oder aktualisieren, sind die jeweils aktuelle Release Notes anzupassen. Diese sind unter "Documentation/Release Notes" zu finden. Des Weiteren ist ggf. das Handbuch anzupassen.