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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    /// <summary>
    /// Contains tests for the <see cref="Operation"/> class.
    /// </summary>
    [TestClass()]
    public class OperationTests
    {
        #region Test methods

        [TestMethod()]
        public void GetCustomDataTest()
        {
            Operation operation = new Operation();
            operation.CustomData["1"] = 1;

            Assert.AreEqual(1, operation.GetCustomData<int>("1"));
            Assert.AreEqual(null, operation.GetCustomData<string>("DoesNotExist"));
        }

        #endregion
    }
}