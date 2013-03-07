using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    /// <summary>
    /// Contains tests for the <see cref="FDResourceConfiguration"/> class.
    /// </summary>
    [TestClass()]
    public class OperationLoopCollectionTests
    {
        #region Test methods

        [TestMethod()]
        public void DontAddDoubles()
        {
            OperationLoopCollection coll = new OperationLoopCollection();
            coll.Add("1");
            coll.Add("1");

            Assert.AreEqual(1, coll.Count);

            coll.Add("2");
            coll[1] = "1";

            Assert.AreEqual(coll[0], "1");
            Assert.AreEqual(coll[1], "2");
        }

        [TestMethod()]
        public void DontAddEmptyLoops()
        {
            OperationLoopCollection coll = new OperationLoopCollection();
            coll.Add(null);
            coll.Add("");

            Assert.AreEqual(0, coll.Count);
        }

        [TestMethod()]
        public void DontAddWhitespaces()
        {
            OperationLoopCollection coll = new OperationLoopCollection();
            coll.Add("   I am whitespaced    ");

            Assert.IsFalse(coll[0].StartsWith(" "));
        }

        #endregion
    }
}
