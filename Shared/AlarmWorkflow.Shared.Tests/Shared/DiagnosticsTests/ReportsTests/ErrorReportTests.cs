using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AlarmWorkflow.Shared.Diagnostics.Reports;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AlarmWorkflow.Shared.Tests.Shared.CoreTests
{
    /// <summary>
    /// Contains tests for the <see cref="ErrorReport"/> class.
    /// </summary>
    [TestClass()]
    public class ErrorReportTests
    {
        /// <summary>
        /// Marks error reports that can be deleted safely.
        /// </summary>
        private const string TestComponent = "AWTestsCanBeDeleted";

        [TestMethod()]
        public void CreateErrorReportBasic()
        {
            ErrorReport report = CreateBasicErrorReport();

            string reportXml = report.Serialize();
            report = ErrorReport.Deserialize(reportXml);

        }

        private static ErrorReport CreateBasicErrorReport()
        {
            InvalidOperationException rootException = CreateTestException();

            ErrorReport report = new ErrorReport(rootException);
            report.Timestamp = DateTime.UtcNow;
            return report;
        }

        private static InvalidOperationException CreateTestException()
        {
            ArgumentException innerException = new ArgumentException("Some argument was wrong.", "someArgument");
            InvalidOperationException rootException = new InvalidOperationException("This action is not allowed!", innerException);
            return rootException;
        }

        [TestMethod()]
        public void CreateErrorReportWithErrorReportManager()
        {
            InvalidOperationException rootException = CreateTestException();

            int countBefore = ErrorReportManager.GetErrorReportsCount();

            ErrorReport report = ErrorReportManager.CreateErrorReport(rootException, TestComponent);
            Assert.IsFalse(string.IsNullOrWhiteSpace(report.ReportFileName));
            Assert.IsTrue(File.Exists(report.ReportFileName));

            int countNow = ErrorReportManager.GetErrorReportsCount();
            Assert.AreNotEqual(countBefore, countNow);

            File.Delete(report.ReportFileName);
            countNow = ErrorReportManager.GetErrorReportsCount();
            Assert.IsFalse(File.Exists(report.ReportFileName));

            Assert.AreEqual(countBefore, countNow);
        }

        [TestMethod()]
        public void GetNewestReports()
        {
            InvalidOperationException rootException = CreateTestException();

            ErrorReport report = ErrorReportManager.CreateErrorReport(rootException, TestComponent);
            IList<ErrorReport> reports = ErrorReportManager.GetNewestReports(TimeSpan.FromMinutes(10.0d), 0).ToList();

            Assert.AreNotEqual(0, reports.Count);

            // Verify that the entries are sorted
            DateTime previous = DateTime.Now;
            foreach (ErrorReport item in reports)
            {
                if (item.Timestamp > previous)
                {
                    Assert.Fail("Unsorted item detected!");
                }
                previous = item.Timestamp;
            }

        }
    }
}
