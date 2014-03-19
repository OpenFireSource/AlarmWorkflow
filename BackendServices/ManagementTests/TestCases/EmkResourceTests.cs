using AlarmWorkflow.BackendService.ManagementContracts.Emk;
using AlarmWorkflow.Shared.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Test.Management.TestCases
{
    [TestClass]
    public class EmkResourceTests
    {
        [TestMethod]
        public void Equals_Positive_Test()
        {
            EmkResource resourceA = new EmkResource()
            {
                SiteAlias = "ABC",
                ResourceAlias = "99/9",
                DisplayName = "99.9",
            };

            EmkResource resourceB = new EmkResource()
            {
                SiteAlias = "ABC",
                ResourceAlias = "99/9",
                DisplayName = "99.9",
            };

            Assert.IsTrue(resourceA.Equals(resourceB));
        }

        [TestMethod]
        public void Equals_Negative_Test()
        {
            EmkResource resourceA = new EmkResource()
            {
                SiteAlias = "ABC",
                ResourceAlias = "99/9",
                DisplayName = "99.9",
            };

            EmkResource resourceB = new EmkResource()
            {
                SiteAlias = "ABC",
                ResourceAlias = "xx/9",
                DisplayName = "99.9",
            };

            Assert.IsFalse(resourceA.Equals(resourceB));
        }

        [TestMethod]
        public void IsMatch_Positive_Test()
        {
            EmkResource emk = new EmkResource()
            {
                SiteAlias = "ABC",
                ResourceAlias = "99/9",
                DisplayName = "99.9",
            };

            OperationResource opRes = new OperationResource()
            {
                FullName = "1.2.3 ABC 99/9 HLF",
            };

            Assert.IsTrue(emk.IsMatch(opRes));
        }

        [TestMethod]
        public void IsMatch_Negative_Test()
        {
            EmkResource emk = new EmkResource()
            {
                SiteAlias = "ABC",
                ResourceAlias = "99/9",
                DisplayName = "99.9",
            };

            OperationResource opRes = new OperationResource()
            {
                FullName = "1.2.3 XYZ 99/9 HLF",
            };

            Assert.IsFalse(emk.IsMatch(opRes));
        }
    }
}
