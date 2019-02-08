using System;
using NUnit.Framework;
using AlarmWorkflow.Shared.Extensibility;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Parser.Library.Test
{
    [TestFixture(TestName = "ILS Schweinfurt Test")]
    [SetCulture("de-DE")]
    public class IlsSchweinfurtTest
    {
        [Test]
        public void Normal()
        {
            IParser p = new ILSSchweinfurtParser();
            Operation o = p.Parse(new[] { "--- FAX ------ FAX ------ FAX ------ FAX ------ v12 --- ", "Absender : ILS Schweinfurt ", "Telefon : +49-9721-4753 0 ", "Zeit : 01.01.2017 11:11:55 ", "Termin : ", "Einsatzn. : S 6.2 123456 11 ", "--------- MITTEILER ----------------------------------- ", "Name : ILS SHL ", "--------- EINSATZORT ---------------------------------- ", "Straße : Bahnhofsstraße ", "Haus-Nr. : 18 ", "Abschnitt : ", "Ortsteil : Entenhausen ", "Ort : 12345 Entenhausen ", "Koordinate : X: 4456969 y: 5340941 - System Gauß-Krüger Zone 4 ", "Objekt : Halle", "Station : ", "Einsatzplan: ", "Schlagwort : Überörtliche Hilfe ", "Stichwort B: ", "Stichwort T: ", "Stichwort S: ÜBERÖRTLICHER EINSATZ ", "Stichwort I: ", "Stichwort R: ", "--------- EINSATZMITTEL ------------------------------- ", "Name : Test", "--------- BEMERKUNG ----------------------------------- ", "Brand", "--------- TEXTBAUSTEIN -------------------------------- ", "---------- HINWEISE ----------------------------------- " });
            Assert.AreEqual("S 6.2 123456 11", o.OperationNumber);
            Assert.AreEqual("Entenhausen", o.Einsatzort.City);
            Assert.AreEqual(48.2, o.Einsatzort.GeoLatitude, 0.02);
            Assert.AreEqual(11.42, o.Einsatzort.GeoLongitude, 0.02);
        }
    }
}
