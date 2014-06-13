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

using System.IO;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.ObjectExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.ObjectExpressionsTests
{
    [TestClass()]
    public class ExtendedObjectExpressionFormatterTests : TestClassBase
    {
        public class Mock
        {
            public int NumericValue { get; set; }

            public Mock()
            {
                NumericValue = 42;
            }
        }

        /// <summary>
        /// Performs a basic script invocation.
        /// </summary>
        [TestMethod()]
        public void BasicExtendedScriptTest()
        {
            Mock mock = new Mock();
            string path = GetTestDataFilePath("Shared\\ObjectExpressionsTests\\Scripts\\BasicExtendedScriptTest.cs");

            ExtendedObjectExpressionFormatter<Mock> formatter = new ExtendedObjectExpressionFormatter<Mock>();
            string value = formatter.ToString(mock, "{$cs=" + path + "}");

            Assert.AreEqual("42", value);
            Assert.IsFalse(formatter.HasError);
        }

        /// <summary>
        /// Asserts that all errors are thrown correctly.
        /// </summary>
        [TestMethod()]
        public void CheckErrorsTest()
        {
            ToStringAndAssertException("Err_CompilationFailed.cs", CustomScriptExecutionException.Reason.CompilationFailed);
            ToStringAndAssertException("I DO NOT EXIST.cs", CustomScriptExecutionException.Reason.ScriptFileNotFound);
            ToStringAndAssertException("Err_NotExOneType.cs", CustomScriptExecutionException.Reason.NotExactlyOneExportedTypeFound);
            ToStringAndAssertException("Err_FuncWrongSig1.cs", CustomScriptExecutionException.Reason.ScriptFunctionMethodHasWrongSignature);
            ToStringAndAssertException("Err_FuncWrongSig2.cs", CustomScriptExecutionException.Reason.ScriptFunctionMethodHasWrongSignature);
            ToStringAndAssertException("Err_FuncNotFound.cs", CustomScriptExecutionException.Reason.ScriptFunctionNotFound);
            ToStringAndAssertException("Err_InvokeFailed.cs", CustomScriptExecutionException.Reason.ScriptInvocationException);
        }

        private void ToStringAndAssertException(string fileName, CustomScriptExecutionException.Reason reason)
        {
            string path = GetTestDataFilePath(string.Format("Shared\\ObjectExpressionsTests\\Scripts\\{0}", fileName));

            Mock mock = new Mock();
            ExtendedObjectExpressionFormatter<Mock> formatter = new ExtendedObjectExpressionFormatter<Mock>();
            string value = formatter.ToString(mock, "{$cs=" + path + "}");

            Assert.IsTrue(formatter.HasError);
            Assert.AreEqual(reason, formatter.Error.FailureReason);
        }

        /// <summary>
        /// Calls the script, which returns a macro (1st pass), which is then parsed (2nd pass).
        /// </summary>
        [TestMethod()]
        public void SecondPassScriptTest()
        {
            Mock mock = new Mock();
            string wd = Utilities.GetWorkingDirectory();
            string path = Path.Combine(wd, "Shared\\ObjectExpressionsTests\\Scripts\\SecondPassScriptTest.cs");

            ExtendedObjectExpressionFormatter<Mock> formatter = new ExtendedObjectExpressionFormatter<Mock>();
            string value = formatter.ToString(mock, "{$cs=" + path + "}");

            Assert.AreEqual("The holy numeric value is: 42!", value);
            Assert.IsFalse(formatter.HasError);
        }

        /// <summary>
        /// Runs a script on an <see cref="Operation"/>, which is a large use-case.
        /// Also tests the case that two same referenced assemblies are added.
        /// </summary>
        [TestMethod()]
        public void OperationScriptTest()
        {
            Operation operation = new Operation();
            operation.Id = 42;

            string wd = Utilities.GetWorkingDirectory();
            string path = Path.Combine(wd, "Shared\\ObjectExpressionsTests\\Scripts\\OperationScriptTest.cs");

            ExtendedObjectExpressionFormatter<Operation> formatter = new ExtendedObjectExpressionFormatter<Operation>();
            string value = formatter.ToString(operation, "{$cs=" + path + "}");

            Assert.AreEqual(operation.Id.ToString(), value);
            Assert.IsFalse(formatter.HasError);
        }
    }
}
