using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.Diagnostics.Reports
{
    /// <summary>
    /// Represents the main error report manager, which stores and manages <see cref="ErrorReport"/>s on the local system.
    /// </summary>
    public static class ErrorReportManager
    {
        #region Constants

        private static readonly string ErrorReportPath = Path.Combine(Utilities.GetLocalAppDataFolderPath(), "ErrorReports");
        private static readonly string ErrorReportArchivePath = Path.Combine(ErrorReportPath, "Archive");
        private const string ErrorReportPathTemplate = "{0}_{1}.{2}";
        private const string ErrorReportExtension = "erx";

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new error report based on the given exception.
        /// </summary>
        /// <param name="exception">The exception to base the <see cref="ErrorReport"/> on.</param>
        /// <param name="componentName">The name of the component. Should be set to allow easier investigation.</param>
        /// <returns>The created <see cref="ErrorReport"/>. -or- null, if the exception parameter was null.</returns>
        public static ErrorReport CreateErrorReport(Exception exception, string componentName)
        {
            if (exception == null)
            {
                return null;
            }

            ErrorReport report = new ErrorReport(exception);
            report.Timestamp = DateTime.UtcNow;
            report.SourceComponentName = componentName;

            StoreErrorReport(report);

            return report;
        }

        private static void StoreErrorReport(ErrorReport report)
        {
            try
            {
                EnsureErrorReportsDirectoryExists();

                string componentNamePart = string.IsNullOrWhiteSpace(report.SourceComponentName) ? "UnknownComponent" : report.SourceComponentName.Trim();
                string fileName = string.Format(ErrorReportPathTemplate, componentNamePart, report.Timestamp.ToLocalTime().ToString("yyyyMMddHHmmssffff"), ErrorReportExtension);
                string path = Path.Combine(ErrorReportPath, fileName);

                File.WriteAllText(path, report.Serialize());

                report.ReportFileName = path;
            }
            catch (Exception)
            {
                // This is an error by itself.
                // TODO: Handle exception carefully. We don't want endless cycles.
            }
        }

        private static void EnsureErrorReportsDirectoryExists()
        {
            if (!Directory.Exists(ErrorReportPath))
            {
                Directory.CreateDirectory(ErrorReportPath);
            }
        }

        /// <summary>
        /// Looks up and returns the amount of all (new) error report files in the report directory.
        /// </summary>
        /// <returns>The amount of all (new) error report files in the report directory.</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">The error report directory did not exist.</exception>
        public static int GetErrorReportsCount()
        {
            DirectoryInfo dir = GetErrorReportDirectory();

            return dir.GetFiles("*." + ErrorReportExtension, SearchOption.TopDirectoryOnly).Length;
        }

        /// <summary>
        /// Returns the <see cref="DirectoryInfo"/>-instance of the ErrorReport-root directory.</summary>
        /// <exception cref="System.IO.DirectoryNotFoundException">The error report directory did not exist.</exception>
        private static DirectoryInfo GetErrorReportDirectory()
        {
            DirectoryInfo dir = new DirectoryInfo(ErrorReportPath);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(Resources.ErrorReportDirectoryNotFoundException);
            }
            return dir;
        }

        /// <summary>
        /// Retrieves the newest reports in the error report-directory that match the given criteria.
        /// Result set is ordered from newest report to oldest.
        /// </summary>
        /// <param name="maxAge">The maximum age of the reports. Use null to return all error reports.</param>
        /// <param name="maxCount">The maximum amount of error reports to return. Use 0 (zero) or less to return all error reports.</param>
        /// <returns>The newest reports in the error report-directory that match the given criteria.</returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">The error report directory did not exist.</exception>
        public static IEnumerable<ErrorReport> GetNewestReports(TimeSpan? maxAge, int maxCount)
        {
            DirectoryInfo dir = GetErrorReportDirectory();

            int actualCount = 0;
            foreach (FileInfo file in dir
                .GetFiles("*." + ErrorReportExtension, SearchOption.TopDirectoryOnly)
                .Where(fi => maxAge.HasValue ? fi.CreationTimeUtc >= (DateTime.UtcNow - maxAge.Value) : true)
                .OrderByDescending(fi => fi.CreationTimeUtc))
            {
                ErrorReport report = null;
                try
                {
                    XDocument doc = XDocument.Parse(File.ReadAllText(file.FullName));
                    report = ErrorReport.Deserialize(doc.ToString());
                }
                catch (Exception)
                {
                    // TODO: Handling if a file could not be parsed.
                }

                if (report == null)
                {
                    continue;
                }

                yield return report;
                actualCount++;

                if (maxCount > 0 && (actualCount >= maxCount))
                {
                    yield break;
                }
            }
        }

        #endregion
    }
}
