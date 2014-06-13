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

        [TestMethod()]
        public void FixIssueWithNullEntries()
        {
            ReplaceDictionary dict = new ReplaceDictionary();
            dict.Pairs.Add(string.Empty, null);

            // They ought to return just the input string with no errors.
            dict.ReplaceInString(null);
            dict.ReplaceInString("");
            dict.ReplaceInString("abcdefg");
        }
    }
}