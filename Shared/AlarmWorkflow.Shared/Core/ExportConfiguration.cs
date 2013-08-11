using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Configures which exports for a given type shall be enabled.
    /// </summary>
    [DebuggerDisplay("Type = {ExportType} (Count = {Exports.Count})")]
    public sealed class ExportConfiguration : IStringSettingConvertible
    {
        #region Properties

        /// <summary>
        /// Gets the list of configured exports.
        /// </summary>
        public List<ExportEntry> Exports { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportConfiguration"/> class.
        /// </summary>
        public ExportConfiguration()
        {
            Exports = new List<ExportEntry>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Enumerates through all configured exports and returns the names of all enabled exports.
        /// </summary>
        /// <returns>The names of all enabled exports.</returns>
        public IList<string> GetEnabledExports()
        {
            List<string> exports = new List<string>();
            foreach (var export in Exports.Where(exp => exp.IsEnabled))
            {
                exports.Add(export.Name);
            }
            return exports;
        }

        /// <summary>
        /// Parses an XML-content and returns the <see cref="ExportConfiguration"/> from it.
        /// </summary>
        /// <param name="value">The XML-contents to parse.</param>
        /// <returns></returns>
        public static ExportConfiguration Parse(string value)
        {
            ExportConfiguration configuration = new ExportConfiguration();
            ((IStringSettingConvertible)configuration).Convert(value);

            return configuration;
        }

        /// <summary>
        /// Creates the XML-representation of the current instance.
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            return ((IStringSettingConvertible)this).ConvertBack();
        }

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                return;
            }

            XDocument doc = XDocument.Parse(settingValue);

            foreach (var exportE in doc.Root.Elements("Export"))
            {
                ExportEntry evm = new ExportEntry();
                evm.Name = exportE.Attribute("Name").Value;
                evm.IsEnabled = bool.Parse(exportE.Attribute("IsEnabled").Value);
                Exports.Add(evm);
            }
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            XElement rootE = new XElement("ExportConfiguration");
            // Write only the enabled exports
            foreach (ExportEntry export in Exports.Where(e => e.IsEnabled))
            {
                XElement exportE = new XElement("Export");
                exportE.Add(new XAttribute("Name", export.Name));
                exportE.Add(new XAttribute("IsEnabled", export.IsEnabled));
                rootE.Add(exportE);
            }

            doc.Add(rootE);
            return doc.ToString();
        }

        #endregion

        /// <summary>
        /// Configures one export.
        /// </summary>
        [DebuggerDisplay("Name = {Name}, IsEnabled = {IsEnabled}")]
        public sealed class ExportEntry
        {
            /// <summary>
            /// Gets/sets the name (alias) of the export.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Gets/sets whether or not this export is enabled.
            /// </summary>
            public bool IsEnabled { get; set; }
        }
    }
}
