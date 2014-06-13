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
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Diagnostics.Reports
{
    /// <summary>
    /// Represents an error report, which was created based on an exception that has occurred.
    /// </summary>
    public sealed class ErrorReport
    {
        #region Properties

        /// <summary>
        /// Gets the error report's file name on the disk.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This is set by the <see cref="ErrorReportManager"/>. If this is not set (<c>null</c>) after creating the error via the manager,
        /// then an error has occurred while trying to save the report to the disk.</remarks>
        public string ReportFileName { get; internal set; }
        /// <summary>
        /// Gets the <see cref="Exception"/>-instance that this error report is based on.
        /// </summary>
        public ExceptionDetail Exception { get; private set; }
        /// <summary>
        /// Gets/sets the timestamp when this exception occurred.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Gets/sets the name of the component where this error was created in.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>This should be set to allow easier investigation.
        /// Possible values could be, for example, "Windows Service", "Windows UI", "Configuration".</remarks>
        public string SourceComponentName { get; set; }
        /// <summary>
        /// Gets whether the source exception has caused the CLR to terminate.
        /// </summary>
        public bool IsTerminating { get; internal set; }

        #endregion

        #region Constructors

        private ErrorReport()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReport"/> class.
        /// </summary>
        /// <param name="exception">The exception object to base this error report on. Must not be null.</param>
        public ErrorReport(Exception exception)
            : this()
        {
            Assertions.AssertNotNull(exception, "exception");
            this.Exception = new ExceptionDetail(exception);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an XML-representation of this instance, ready to be saved to disk.
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            XDocument doc = new XDocument();
            XElement root = new XElement("ErrorReport");
            root.Add(new XElement("Timestamp", Timestamp));
            root.Add(new XElement("ComponentName", SourceComponentName));
            root.Add(new XElement("IsTerminating", IsTerminating));
            root.Add(Exception.Serialize());

            doc.Add(root);
            return doc.ToString();
        }

        /// <summary>
        /// Parses the given XML-representation and creates an <see cref="ErrorReport"/> off of it.
        /// </summary>
        /// <param name="xml">The XML-representation to convert.</param>
        /// <returns></returns>
        public static ErrorReport Deserialize(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement root = doc.Root;

            ErrorReport report = new ErrorReport();
            report.Timestamp = DateTime.Parse(root.Element("Timestamp").Value).ToUniversalTime();
            report.SourceComponentName = root.Element("ComponentName").Value;
            report.IsTerminating = bool.Parse(root.Element("IsTerminating").Value);
            report.Exception = ExceptionDetail.Deserialize(root.Element("ExceptionDetail"));

            return report;
        }

        #endregion
    }
}