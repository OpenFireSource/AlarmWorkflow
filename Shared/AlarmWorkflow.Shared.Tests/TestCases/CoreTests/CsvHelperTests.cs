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

using System.Collections.Generic;
using System.Linq;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    [TestClass()]
    public class CsvHelperTests
    {
        [TestMethod()]
        public void CheckBasicParsing()
        {
            const string csvString = "this;is;a;quotated;string";

            string[] columns = CsvHelper.FromCsvLine(csvString).ToArray();

            Assert.AreEqual(5, columns.Length);
        }

        [TestMethod()]
        public void CheckBasicFormatting()
        {
            object[] values = new object[] { "this", "is", "an", "array", 1337, 4711 };

            string result = CsvHelper.ToCsvLine(values);

            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            Assert.AreNotEqual(0, result.Length);
            Assert.AreEqual(6, result.Split(CsvHelper.SeparatorChar).Length);
        }

        [TestMethod()]
        public void CheckQuotationMarkParsing()
        {
            const string csvString = "\"this\";\"is\";\"a\";\"quotated\";\"string\"";

            string[] columns = CsvHelper.FromCsvLine(csvString).ToArray();

            Assert.AreEqual(5, columns.Length);
            Assert.IsFalse(columns.Any(col => col.Contains('"')));
        }

        [TestMethod()]
        public void CheckSeparatorAndQuotationWithinCsvParsing()
        {
            Dictionary<string, string[]> useCases = new Dictionary<string, string[]>();
            useCases.Add("\"this\";\"is;a\";\"quotated\";\"string\"", new string[] { "this", "is;a", "quotated", "string" });
            useCases.Add("\"this\";\"is;a\"hello dude;\"quotated\";\"string\"", new string[] { "this", "is;ahello dude", "quotated", "string" });
            useCases.Add("\"1\"2;3;4;5", new string[] { "1", "23", "4", "5" });
            useCases.Add("\"1\";2\";3\";4\"\";5;", new string[] { "1", "2;3", "4", "5" });

            foreach (var item in useCases)
            {
                string[] columns = CsvHelper.FromCsvLine(item.Key).ToArray();

                Assert.AreEqual(item.Value.Length, columns.Length);
            }
        }
    }
}