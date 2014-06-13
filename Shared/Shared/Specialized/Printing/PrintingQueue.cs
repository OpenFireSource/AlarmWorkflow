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
using AlarmWorkflow.Shared.Properties;

namespace AlarmWorkflow.Shared.Specialized.Printing
{
    /// <summary>
    /// Represents a single configuration printing queue, that is a description of a printing task, determining how something is printed.
    /// </summary>
    public sealed class PrintingQueue : IEquatable<PrintingQueue>
    {
        #region Fields

        private string _name;
        private int _copyCount = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets the name of the printing queue.
        /// The names are case-insensitive.
        /// </summary>
        /// <exception cref="System.ArgumentException">Value is null or empty.</exception>
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(Resources.PrintingQueueNameMustNotBeEmpty, "Name");
                }
                _name = value;
            }
        }

        /// <summary>
        /// Gets/sets the print server to use.
        /// If this is null or empty, the local print server is used.
        /// </summary>
        public string PrintServer { get; set; }
        /// <summary>
        /// Gets/sets the name of the printer to use.
        /// If this is null or empty, the default printer for the specified <see cref="PrintServer"/> is used.
        /// </summary>
        public string PrinterName { get; set; }
        /// <summary>
        /// Gets/sets the amount of copies to print.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Value is less than 1.</exception>
        public int CopyCount
        {
            get { return _copyCount; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(Resources.PrintingQueueCopyCountMustBeGreaterThanZero);
                }
                _copyCount = value;
            }
        }
        /// <summary>
        /// Gets/sets whether or not this printing queue is enabled.
        /// If a printing queue is disabled, no printing will be done.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets/sets whether the "alternative" method of printing multiple copies should be used.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Enabling this option instructs the responsible printer to not use the driver's intended method,
        /// but instead repeating the printing one copy at a time. This is to work around a possible bug in the OS or the drivers
        /// that report a higher copy count than they can actually handle.</remarks>
        public bool UseAlternativeCopyingMethod { get; set; }

        /// <summary>
        /// Returns true if the configuration represents the localhost print server.
        /// This is the case if there is no print server specified.
        /// </summary>
        public bool IsLocalPrintServer
        {
            get { return string.IsNullOrWhiteSpace(PrintServer); }
        }

        /// <summary>
        /// Returns true if the configuration represents the default printer for the print server.
        /// This is the case if there is no printer name specified.
        /// </summary>
        public bool IsDefaultPrinter
        {
            get { return string.IsNullOrWhiteSpace(PrinterName); }
        }

        /// <summary>
        /// Returns whether or not this instance is properly configured.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>An instance is properly configured, if:
        /// It is a local print server (no <see cref="PrintServer"/> specified) and <see cref="PrinterName"/> either has a value or not.
        /// It is a remote print server (<see cref="PrintServer"/> has a valid value, not empty), and <see cref="PrinterName"/> is specified (not empty).</remarks>
        public bool IsValid
        {
            get
            {
                if (IsLocalPrintServer)
                {
                    return true;
                }
                else
                {
                    return !string.IsNullOrWhiteSpace(this.PrinterName);
                }
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueue"/> class.
        /// </summary>
        public PrintingQueue()
        {
            Name = string.Format("Unnamed printing queue {0}", Guid.NewGuid());
            IsEnabled = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assembles and returns the printer name.
        /// </summary>
        /// <returns>If the <see cref="PrintServer"/>-property is set, it prefixes the printer in a format such as "\\(PrintServer)\",
        /// so that the full name equals to "\\(PrintServer)\(PrinterName)". Otherwise, only <see cref="PrinterName"/> is returned.</returns>
        public string GetPrinterName()
        {
            if (!string.IsNullOrWhiteSpace(PrintServer))
            {
                return string.Format(@"\\{0}\{1}", PrintServer, PrinterName);
            }
            return PrinterName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is PrintingQueue)
            {
                return Equals((PrintingQueue)obj);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        #region IEquatable<PrintingQueue> Members

        /// <summary>
        /// Returns whether or not this instance equals an other instance, value-wise.
        /// Two <see cref="PrintingQueue"/>-instances are considered equal if their names are equal (case-insensitive).
        /// </summary>
        /// <param name="other">The other instance to check for equality.</param>
        /// <returns>Whether or not this instance equals an other instance, value-wise.</returns>
        public bool Equals(PrintingQueue other)
        {
            return string.Equals(other.Name, this.Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}