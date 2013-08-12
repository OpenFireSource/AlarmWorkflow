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

using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.ObjectExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.ObjectExpressionsTests
{
    [TestClass]
    public class OperationExpressionTests
    {
        [TestMethod]
        public void FormatOperationRegular()
        {
            Operation operation = new Operation();
            operation.Id = 42;
            operation.Comment = "Brand!!!";

            string format = "{Id}, {Comment}";
            string resultExpected = string.Format("{0}, {1}", operation.Id, operation.Comment);
            string result = ObjectFormatter.ToString(operation, format);

            Assert.AreEqual(resultExpected, result);
        }

        [TestMethod]
        public void FormatOperationRegular2()
        {
            Operation operation = new Operation();
            operation.Id = 42;
            operation.Comment = "Brand!!!";

            string format = "{Id}, {MichGibtsNicht}";
            string resultExpected = string.Format("{0}, {1}", operation.Id, "");
            string result = ObjectFormatter.ToString(operation, format);

            Assert.AreEqual(resultExpected, result);
        }

        [TestMethod]
        public void FormatOperationCustomData()
        {
            string cdexpr1 = "ort";
            string cdexpr1value = "12345";
            string cdexpr2 = "myfunny.little.weird.expression";
            int cdexpr2value = 42;
            string resultExpected = string.Format("{0} - {1}", cdexpr1value, cdexpr2value);

            Operation operation = new Operation();
            operation.CustomData[cdexpr1] = cdexpr1value;
            operation.CustomData[cdexpr2] = cdexpr2value;

            string format = string.Format("{{{0}}} - {{{1}}}", cdexpr1, cdexpr2);
            string result = operation.ToString(format);

            Assert.AreEqual(resultExpected, result);
        }

        [TestMethod]
        public void FormatOperationCustomDataFail()
        {
            Operation operation = new Operation();
            operation.Id = 42;
            operation.Comment = "Brand!!!";

            string format = "{Id}, {MichGibtsNicht}";
            string resultExpected = string.Format("{0}, {1}", operation.Id, "");
            string result = operation.ToString(format);

            Assert.AreEqual(resultExpected, result);
        }
    }
}