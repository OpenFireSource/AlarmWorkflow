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
