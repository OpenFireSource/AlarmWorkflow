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

using AlarmWorkflow.Parser.Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Parsers
{
    /// <summary>
    /// Tests the functionality of the ParserUtility
    /// </summary>
    [TestClass()]
    public class ParserUtilityTest
    {
        /// <summary>
        /// Tests the splitting of the street line in multiple layouts.
        /// </summary>
        [TestMethod]
        public void StreetLineTest()
        {
            string street, streetnumber, appendix;

            // Layout like on faxes by ILS FFB
            string line = "Musterstraße 2 a RH";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("2 a", streetnumber);
            Assert.AreEqual("RH", appendix);

            line = "Musterstraße 2 a";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("2 a", streetnumber);
            Assert.AreEqual(string.Empty, appendix);

            line = "Musterstraße";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("1", streetnumber);
            Assert.AreEqual(string.Empty, appendix);

            line = "Musterstraße 2 1.OG";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("2", streetnumber);
            Assert.AreEqual("1.OG", appendix);

            line = "1.2 A8 Musterstadt > Entenhausen 123";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("1.2 A8 Musterstadt > Entenhausen", street);
            Assert.AreEqual("123", streetnumber);
            Assert.AreEqual(string.Empty, appendix);

            // Layout like on faxes by ILS Traunstein or may others
            line = "Musterstraße Haus-Nr.: 2 a RH";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("2 a", streetnumber);
            Assert.AreEqual("RH", appendix);

            line = "Musterstraße Haus-Nr.: 2 a";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("2 a", streetnumber);
            Assert.AreEqual(string.Empty, appendix);

            line = "Musterstraße Haus-Nr.: 2 1.OG";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("Musterstraße", street);
            Assert.AreEqual("2", streetnumber);
            Assert.AreEqual("1.OG", appendix);

            line = "A8 A Musterstadt > Entenhausen Haus-Nr.: 123 --- km BAB";
            ParserUtility.AnalyzeStreetLine(line, out street, out streetnumber, out appendix);

            Assert.AreEqual("A8 A Musterstadt > Entenhausen", street);
            Assert.AreEqual("123 --- km BAB", streetnumber);
            Assert.AreEqual(string.Empty, appendix);
        }
    }
}
