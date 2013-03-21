using AlarmWorkflow.Shared.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    /// <summary>
    /// Contains tests for the <see cref="ReplaceDictionary"/> class.
    /// </summary>
    [TestClass()]
    public class ReplaceDictionaryTests
    {
        [TestMethod()]
        public void CheckRegexMatchTest()
        {
            string replacementChar = "!";

            ReplaceDictionary dict = new ReplaceDictionary();
            dict.InterpretAsRegex = true;
            dict.Pairs.Add(@"\bword\b", replacementChar);

            string input = "Come word get word some word";

            string result = dict.ReplaceInString(input);

            Assert.IsTrue(result.Contains(replacementChar));
        }
    }
}
