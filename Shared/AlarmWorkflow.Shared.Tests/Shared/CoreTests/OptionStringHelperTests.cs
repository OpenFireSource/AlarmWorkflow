using System.Collections.Generic;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    [TestClass()]
    public class OptionStringHelperTests
    {
        [TestMethod()]
        public void CheckSkipInvalidPairs()
        {
            const string optionString = "key1=value1;key2;key3=value3;";

            IDictionary<string, string> result = OptionStringHelper.GetAsPairs(optionString);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("value1", result["key1"]);
            Assert.AreEqual("value3", result["key3"]);
        }
    }
}
