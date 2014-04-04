Dieses Dokument soll als Anlaufstelle für die Entwicklung von **AlarmWorkflow** dienen.

# Allgemeines

## Entwicklungsumgebung

Eine Entwicklungsumgebung ist notwendig, damit Sie den Quellcode von AlarmWorkflow kompilieren können. Im Projekt wird **Microsoft Visual Studio 2010** verwendet.
Hiervon gibt es eine sog. **Express**-Version, welche nicht alle Features der Vollversion bietet, aber kostenlos ist und im Regelfall mehr als ausreicht.
Sie können dieses unter http://www.visualstudio.com/downloads/download-visual-studio-vs#DownloadFamilies_4 in der Version **Visual C-Sharp 2010 Express** herunterladen.

Das von AlarmWorkflow benötigte **Microsoft .Net Framework 4.0** wird automatisch mitinstalliert, falls es noch nicht installiert ist.

## Sourcecode

Es wird empfohlen, die von den Entwicklern zu Verfügung gestellte Version zu verwenden. Sollten Sie jedoch eigenständig Modifikationen vornehmen so besteht die Möglichkeit über GitHub die Sourcecodes zu beziehen.

## Programmierstil

Die Programmierrichtlinien befinden sich in der Datei *CodingGuidelines.de.md*.

## Mitentwickeln/Beitragen

Es ist erwünscht, dass Programmierkundige zu AlarmWorkflow beitragen! Hierfür geht ihr bitte den unter Git üblichen Weg der **Pull requests**.
Informationen dazu gibt es unter: https://help.github.com/articles/using-pull-requests

### Submodules

In Git gibt es die Möglichkeit, externe Repositories einzubeziehen, ohne sie voll integrieren zu müssen. Dies nennt sich "submodule".

Wenn Sie das Repository bereits geklont haben, müssen Sie sich das externe Repository klonen:
- Navigieren Sie in Ihr "AlarmWorkflow" Repository
- Klicken Sie rechts in den Ordner und öffnen das Kontextmenü
- Klicken Sie auf "TortoiseGit --> Submodule update..."
- Wählen Sie "OK"
- Die submodules werden nun geklont und befinden sich danach in Ihrem Repository.

Wenn Sie sich das Repository neu klonen, müssen Sie folgendes beachten:
- Im "Clone"-Dialog von TortoiseGit: Wählen Sie **recursive** aus
- Per Shell: Hängen Sie den Parameter **--recursive** an

Je nach verwendeter Sprache von TortoiseGit heißt der Punkt u. U. anders als hier angegeben.

Wenn die submodules sich ändern, müssen Sie Ihr lokales Repository ebenfalls aktualisieren. Dies funktioniert analog zum ersteren Fall oben.

# Anforderungen an Patches und Pull Requests

## Coding guidelines
Ein Code style ist zwingend erforderlich, damit der Code immer einheitlich und leserlich bleibt. Ein guter, einheitlicher Code Style hilft allen Beteiligten und vor allem auch Neulingen, sich schnell zurechtzufinden, da man immer davon ausgehen kann, dass der Code stets uniform aussieht.

Hierzu sind die Coding Guidelines zu beachten und von Zeit zu Zeit zu besuchen, falls es zu Änderungen kam.
Siehe dazu die Datei *CodingGuidelines.de.md*.

## Contribution guidelines
Es werden gewisse Standards vorgegeben, an die sich Beiträge zu halten haben, damit die Codebasis eine konsistente Qualität behalten kann.

## Codequalität
Zwingende Voraussetzung ist die Einhaltung der Coding guidelines (s.o.). Beiträge, welche diese nicht erfüllen, werden abgelehnt und müssen nachgebessert werden.

## Source branch
Als source branch, also der Zweig, auf dem eure Commits basieren, soll der Zweig "next" dienen.
Dies dient dazu, den Hauptzweig von Änderungen jeglicher Natur freizuhalten, damit auf dem Hauptzweig stets ein stabiler und funktionierender Stand gegeben ist.
Dies soll allerdings **nicht** bedeuten, dass **next** als "Spielwiese" zu betrachten ist! ;-)

## Commit-Umfang
Es ist immer vorzuziehen, einen Commit als "atomare Einheit" zu handhaben. Das bedeutet:
Keine Vermischung von zwei oder mehr Themengebieten, die unabhängig voneinander sind (dies ist im Regelfall möglich, kann aber auch seltene Ausnahmen geben)

## Commit-Nachrichten
Ein Commit ist grundsätzlich analog zu folgendem Beispiel aufzubauen:

> Kurze, aber präzise Beschreibung
>
> (Absatz)
>
> Längere Ausführung, was der Commit macht. Eine Beschreibung ist besonders bei größeren Commits zu verfassen!

## Patches vs. Pull Requests
Bei kleineren Änderungen sind Patches zu bevorzugen, wohingegen größere Arbeiten per PR geteilt werden sollten.

## PR-Commits
Es ist Usus, einen PR erst dann zu pushen, wenn alle enthaltenen Commits auch wirklich sinnvoll sind. Dies ist z.B. bei folgendem PR nicht der Fall:

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