using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Tests.CoreTests
{
    /// <summary>
    /// Contains tests for the <see cref="FDResourceConfiguration"/> class.
    /// </summary>
    [TestClass()]
    public class FDResourceConfigurationTests
    {
        #region Test methods

        /// <summary>
        /// Tests that filtering works as expected.
        /// </summary>
        [TestMethod()]
        public void GetFilteredResourcesTest()
        {
            Operation operation = new Operation();
            operation.Resources.Add(new OperationResource() { FullName = "123749 123879 FF Bayern 40/1 (LF)" });
            operation.Resources.Add(new OperationResource() { FullName = "123746 123879 FF Thüringen 40/1 (LF)" });
            operation.Resources.Add(new OperationResource() { FullName = "123749 123879 FF Bayern 40/2 (LF)" });
            operation.Resources.Add(new OperationResource() { FullName = "123709 123879 FF Baden-Württemberg 30/1 (DLK 23/12)" });
            operation.Resources.Add(new OperationResource() { FullName = "123709 123879 FF Baden-Württemberg 41/2 (LF)" });

            FDResourceConfiguration configuration = new FDResourceConfiguration();
            configuration.FDIdentification = "Bayern";
            configuration.Add(new FDResource() { Identifier = "40/1" });
            configuration.Add(new FDResource() { Identifier = "40/2" });

            OperationResourceCollection filtered = configuration.GetFilteredResources(operation);

            Assert.AreEqual(2, filtered.Count);
        }

        #endregion
    }
}
