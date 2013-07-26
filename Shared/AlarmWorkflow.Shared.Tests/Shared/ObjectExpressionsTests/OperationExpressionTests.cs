using AlarmWorkflow.Shared.Core;
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
