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
    /// Represents the consolidated, serializable details of an <see cref="Exception"/>.
    /// </summary>
    public sealed class ExceptionDetail
    {
        #region Properties

        /// <summary>
        /// Gets/sets the type of the underlying <see cref="Exception"/>.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Gets/sets the exception's message.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Gets/sets the exception's source.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Gets/sets the exception's stack trace.
        /// </summary>
        public string StackTrace { get; set; }
        /// <summary>
        /// Gets/sets the exception's inner exception (if present).
        /// </summary>
        public ExceptionDetail InnerException { get; set; }

        #endregion

        #region Constructors

        private ExceptionDetail()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionDetail"/> class.
        /// </summary>
        /// <param name="exception">The exception object. Must not be null.</param>
        internal ExceptionDetail(Exception exception)
            : this()
        {
            Assertions.AssertNotNull(exception, "exception");

            this.Type = exception.GetType().AssemblyQualifiedName;
            this.Message = exception.Message;
            this.Source = exception.Source;
            this.StackTrace = exception.StackTrace;

            if (exception.InnerException != null)
            {
                this.InnerException = new ExceptionDetail(exception.InnerException);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an XML-representation of this instance, ready to be saved to disk.
        /// </summary>
        /// <returns>An <see cref="XElement"/> with the contents of this instance.</returns>
        internal XElement Serialize()
        {
            XElement element = new XElement("ExceptionDetail");
            element.Add(new XAttribute("Type", this.Type));
            element.Add(new XElement("Message", this.Message));
            element.Add(new XElement("Source", this.Source));
            element.Add(new XElement("StackTrace", this.StackTrace));

            if (this.InnerException != null)
            {
                element.Add(this.InnerException.Serialize());
            }

            return element;
        }

        /// <summary>
        /// Parses the given XML-representation and creates an <see cref="ExceptionDetail"/> off of it.
        /// </summary>
        /// <param name="xml">The XML-representation to convert.</param>
        /// <returns></returns>
        internal static ExceptionDetail Deserialize(XElement xml)
        {
            ExceptionDetail detail = new ExceptionDetail();
            detail.Type = xml.Attribute("Type").Value;
            detail.Message = xml.Element("Message").Value;
            detail.Source = xml.Element("Source").Value;
            detail.StackTrace = xml.Element("StackTrace").Value;

            XElement innerException = xml.Element("ExceptionDetail");
            if (innerException != null)
            {
                detail.InnerException = Deserialize(innerException);
            }

            return detail;
        }

        #endregion
    }
}