using System;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Properties;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Specialized.Printing
{
    /// <summary>
    /// Stores configured <see cref="PrintingQueue"/>s and manages them for convenient access.
    /// </summary>
    public sealed class PrintingQueuesConfiguration : IStringSettingConvertible
    {
        #region Properties

        /// <summary>
        /// Gets the collection of entries in this configuration.
        /// </summary>
        public PrintingQueueCollection Entries { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintingQueuesConfiguration"/> class.
        /// </summary>
        public PrintingQueuesConfiguration()
        {
            Entries = new PrintingQueueCollection();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the <see cref="PrintingQueue"/> that is registered for the given name.
        /// </summary>
        /// <param name="name">The name of the <see cref="PrintingQueue"/> to return. Printing queue names are case-insensitive.</param>
        /// <returns>The <see cref="PrintingQueue"/> that is registered for the given name.
        /// -or- null, if there was no printing queue registered for that name.</returns>
        public PrintingQueue GetPrintingQueue(string name)
        {
            return Entries.FirstOrDefault(pq => string.Equals(pq.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            XDocument doc = XDocument.Parse(settingValue);

            foreach (XElement pqe in doc.Root.Elements("PrintingQueue"))
            {
                // Sanity-check name first to avoid exception.
                string pqName = pqe.TryGetAttributeValue("Name", null);
                if (string.IsNullOrWhiteSpace(pqName))
                {
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.PrintingQueueParseErrorNameIsInvalid);
                    continue;
                }

                PrintingQueue pq = new PrintingQueue();
                pq.Name = pqName;

                pq.PrintServer = pqe.TryGetAttributeValue("PrintServer", null);
                pq.PrinterName = pqe.TryGetAttributeValue("PrinterName", null);
                pq.IsEnabled = pqe.TryGetAttributeValue("IsEnabled", true);

                // Sanity-check copy count to avoid exception.
                int pqCopyCount = pqe.TryGetAttributeValue("CopyCount", 1);
                if (pqCopyCount < 1)
                {
                    pqCopyCount = 1;
                    Logger.Instance.LogFormat(LogType.Warning, this, Resources.PrintingQueueCopyCountMustBeGreaterThanZero, pqCopyCount);
                }
                pq.CopyCount = pqCopyCount;

                this.Entries.Add(pq);
            }
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("PrintingQueuesConfiguration"));

            foreach (PrintingQueue pq in this.Entries)
            {
                XElement pqe = new XElement("PrintingQueue");
                pqe.Add(new XAttribute("Name", pq.Name));
                pqe.Add(new XAttribute("PrintServer", pq.PrintServer ?? string.Empty));
                pqe.Add(new XAttribute("PrinterName", pq.PrinterName ?? string.Empty));
                pqe.Add(new XAttribute("CopyCount", pq.CopyCount));
                pqe.Add(new XAttribute("IsEnabled", pq.IsEnabled));

                doc.Root.Add(pqe);
            }

            return doc.ToString();
        }

        #endregion
    }
}
