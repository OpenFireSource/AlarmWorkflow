using System;
using AlarmWorkflow.Parser.Library;
using AlarmWorkflow.Shared.Extensibility;
using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using NUnit.Framework;

namespace Parsers.Test
{
    [TestFixture(TestName ="ILS FFB Test")]
    [SetCulture("de-DE")]
    public class ILSFFBTest
    {
        [Test]
        public void Highway()
        {
            List<string> s = new List<string> { "INTEGRIERTE LEITSTELLE FÜRSTENFELDBRUCK","Telefon: +49 (8141) 22700-600 Telefax: +49 (8141) 22700-641","ILS Fürstenfeldbruck - ALARMSCHREIBEN FEUERWEHR","E-Nr: T 1.2 123456 1234 ALARM: 04.06.2017 08:30","EINSATZORT","STRAßE: 1.2 A99a AD München-Eschenried > AD München-Allach 102","ABSCHNITT: A99 > Salzburg Landkreisgrenze FFB/DAH > Landkreisgrenze DAH","/M (101.264 bis 103.162)","ORTSTEIL/ORT:","KOORDINATEN: 4456969.33 / 5340941.47","OBJEKT:","EINSATZPLAN:","MELDEBILD: VU","EINSATZSTICHWORT: VU 1","HINWEIS: 1 x Leicht","EINSATZMITTEL:","1.2.1 FFB FF Musterstadt ()","(ALARMSCHREIBEN ENDE)","Auch mit Alarmschreiben zur Übernahme des Einsatzes","den Status 0 (dringender Sprechwunsch) am Digitalfunk verwenden!","Das Alarmschreiben darf nur zum internen Dienstgebrauch verwendet werden.","Der Empfänger hat sicherzustellen, dass unbefugte keinen Zugang zu den","übermittelten Daten erhalten."};
            IParser parser = new ILSFFBParser();
            Operation o = parser.Parse(s.ToArray());
            Assert.AreEqual(new DateTime(2017, 06, 04, 08, 30, 00), o.Timestamp);
            Assert.AreEqual("A99 > Salzburg Landkreisgrenze FFB/DAH > Landkreisgrenze DAH/M (101.264 bis 103.162)", o.Einsatzort.Intersection);
            Assert.AreEqual("A99a AD München-Eschenried > AD München-Allach", o.Einsatzort.Street);
            Assert.AreEqual("102", o.Einsatzort.StreetNumber);
        }

        [Test]
        public void InCity()
        {
            List<string> s = new List<string> { "INTEGRIERTE LEITSTELLE FÜRSTENFELDBRUCK", "Telefon: +49 (8141) 22700-600 Telefax: +49 (8141) 22700-641", "ILS Fürstenfeldbruck - ALARMSCHREIBEN FEUERWEHR", "E-Nr: B 1.2 12345 1234 ALARM: 20.06.2017 08:30", "EINSATZORT", "STRAßE: Mustergasse 7 f KG", "ABSCHNITT: Mustergasse", "ORTSTEIL/ORT: Entenhausen", "KOORDINATEN:", "OBJEKT:", "EINSATZPLAN:", "MELDEBILD: 1.2 Brand Zimmer, Küche / ohne Personengefahr", "EINSATZSTICHWORT: B 3", "HINWEIS:", "EINSATZMITTEL:", "1.2.1 FFB FF Musterstadt ()", "1.2.1 FFB FL Musterstadt 30/1 ()", "(ALARMSCHREIBEN ENDE)", "Auch mit Alarmschreiben zur Übernahme des Einsatzes", "den Status 0 (dringender Sprechwunsch) am Digitalfunk verwenden!", "Das Alarmschreiben darf nur zum internen Dienstgebrauch verwendet werden.", "Der Empfänger hat sicherzustellen, dass unbefugte keinen Zugang zu den", "übermittelten Daten erhalten.", "", "" };
            IParser parser = new ILSFFBParser();
            Operation o = parser.Parse(s.ToArray());
            Assert.AreEqual("Mustergasse", o.Einsatzort.Street);
            Assert.AreEqual("7 f", o.Einsatzort.StreetNumber);
        }
    }
}
