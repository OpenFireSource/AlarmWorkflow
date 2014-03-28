using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Test.Management.TestCases
{
    [TestClass]
    public class EmkResourceCollectionTests
    {
        [TestMethod]
        public void ContainsMatch_Positive_Test()
        {
            EmkResourceCollection collection = new EmkResourceCollection();
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "77/7" });
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "88/8" });
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "99/9" });

            OperationResource opRes = new OperationResource()
            {
                FullName = "1.2.3 ABC 88/8 LF",
            };

            Assert.IsTrue(collection.ContainsMatch(opRes));
        }

        [TestMethod]
        public void ContainsMatch_Negative_Test()
        {
            EmkResourceCollection collection = new EmkResourceCollection();
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "77/7" });
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "88/8" });
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "99/9" });

            OperationResource opRes = new OperationResource()
            {
                FullName = "1.2.3 ABC 66/8 LF",
            };

            Assert.IsFalse(collection.ContainsMatch(opRes));
        }

        [TestMethod]
        public void ContainsMatch_WithInactiveOnes_Negative_Test()
        {
            EmkResourceCollection collection = new EmkResourceCollection();
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "77/7" });
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "88/8", IsActive = false });
            collection.Add(new EmkResource() { SiteAlias = "ABC", ResourceAlias = "99/9" });

            OperationResource opRes = new OperationResource()
            {
                FullName = "1.2.3 ABC 88/8 LF",
            };

            Assert.IsFalse(collection.ContainsMatch(opRes));
        }
    }
}
