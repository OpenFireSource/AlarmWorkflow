﻿// This file is part of AlarmWorkflow.
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

using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// Allgemeine Informationen �ber eine Assembly werden �ber die folgenden 
// Attribute gesteuert. �ndern Sie diese Attributwerte, um die Informationen zu �ndern,
// die mit einer Assembly verkn�pft sind.
using AlarmWorkflow.Shared.Core;

// Durch Festlegen von ComVisible auf "false" werden die Typen in dieser Assembly unsichtbar 
// f�r COM-Komponenten.  Wenn Sie auf einen Typ in dieser Assembly von 
// COM zugreifen m�ssen, legen Sie das ComVisible-Attribut f�r diesen Typ auf "true" fest.
[assembly: ComVisible(false)]

//Um mit dem Erstellen lokalisierbarer Anwendungen zu beginnen, legen Sie 
//<UICulture>ImCodeVerwendeteKultur</UICulture> in der .csproj-Datei
//in einer <PropertyGroup> fest.  Wenn Sie in den Quelldateien beispielsweise Deutsch
//(Deutschland) verwenden, legen Sie <UICulture> auf \"de-DE\" fest.  Heben Sie dann die Auskommentierung
//des nachstehenden NeutralResourceLanguage-Attributs auf.  Aktualisieren Sie "en-US" in der nachstehenden Zeile,
//sodass es mit der UICulture-Einstellung in der Projektdatei �bereinstimmt.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly:ThemeInfo(
    ResourceDictionaryLocation.None, //Speicherort der designspezifischen Ressourcenw�rterb�cher
                             //(wird verwendet, wenn eine Ressource auf der Seite 
                             // oder in den Anwendungsressourcen-W�rterb�chern nicht gefunden werden kann.)
    ResourceDictionaryLocation.SourceAssembly //Speicherort des generischen Ressourcenw�rterbuchs
                                      //(wird verwendet, wenn eine Ressource auf der Seite, in der Anwendung oder einem 
                                      // designspezifischen Ressourcenw�rterbuch nicht gefunden werden kann.)
)]

[assembly: AlarmWorkflowPackage()]
