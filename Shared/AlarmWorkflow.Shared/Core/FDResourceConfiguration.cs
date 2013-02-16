using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Diagnostics;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Contains all resources from a given fire department and provides detailed information
    /// for use by clients (jobs, plug-ins, website etc.).
    /// </summary>
    public sealed class FDResourceConfiguration : Collection<FDResource>, IStringSettingConvertible
    {
        #region Properties

        /// <summary>
        /// Gets/sets the text that is used to refer to resources from this fire department.
        /// This is usually a three-letter abbreviation, i. e. "FFB" or "NEA".
        /// </summary>
        public string FDIdentification { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FDResourceConfiguration"/> struct.
        /// </summary>
        public FDResourceConfiguration()
            : base()
        {
            FDIdentification = String.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Iterates over all <see cref="OperationResource"/>s in the given <see cref="P:Operation.Resource"/>
        /// and returns only those that are configured in this configuration.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> to filter its resources.</param>
        /// <returns>A <see cref="OperationResourceCollection"/>-instance that contains only the resources that are configured in this configuration.</returns>
        public OperationResourceCollection GetFilteredResources(Operation operation)
        {
            Assertions.AssertNotNull(operation, "operation");
            Assertions.AssertNotNull(operation.Resources, "operation.Resources");

            return GetFilteredResources(operation.Resources);
        }

        /// <summary>
        /// Iterates over all <see cref="OperationResource"/>s in the given <see cref="OperationResourceCollection"/>
        /// and returns only those that are configured in this configuration.
        /// </summary>
        /// <param name="operationResourcesCollection">The <see cref="OperationResourceCollection"/> to filter.</param>
        /// <returns>A <see cref="OperationResourceCollection"/>-instance that contains only the resources that are configured in this configuration.</returns>
        public OperationResourceCollection GetFilteredResources(OperationResourceCollection operationResourcesCollection)
        {
            Assertions.AssertNotNull(operationResourcesCollection, "operationResourcesCollection");

            OperationResourceCollection filtered = new OperationResourceCollection();
            foreach (OperationResource item in operationResourcesCollection)
            {
                if (IsMatch(item))
                {
                    filtered.Add(item);
                }
            }

            return filtered;
        }

        private bool IsMatch(OperationResource resource)
        {
            bool containsIdentification = string.IsNullOrWhiteSpace(this.FDIdentification) ? true : resource.FullName.Contains(this.FDIdentification);
            if (!containsIdentification)
            {
                return false;
            }

            if (!this.Items.Any(v => resource.FullName.Contains(v.Identifier)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses an XML-content and returns the <see cref="FDResourceConfiguration"/> from it.
        /// </summary>
        /// <param name="value">The XML-contents to parse.</param>
        /// <returns></returns>
        public static FDResourceConfiguration Parse(string value)
        {
            FDResourceConfiguration configuration = new FDResourceConfiguration();
            ((IStringSettingConvertible)configuration).Convert(value);

            return configuration;
        }

        #endregion

        #region IStringSettingConvertible Members

        void IStringSettingConvertible.Convert(string settingValue)
        {
            if (string.IsNullOrWhiteSpace(settingValue))
            {
                return;
            }

            try
            {
                ConvertCore(settingValue);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogFormat(LogType.Error, this, Properties.Resources.FDResourceConfigurationParseError);
                Logger.Instance.LogException(this, ex);
            }
        }

        private void ConvertCore(string settingValue)
        {
            XDocument doc = XDocument.Parse(settingValue);
            this.FDIdentification = doc.Root.Attribute("FDIdentification").Value;
            foreach (var exportE in doc.Root.Elements("Item"))
            {
                FDResource rdr = new FDResource();
                rdr.Identifier = exportE.Attribute("Id").Value;
                rdr.DisplayName = exportE.Attribute("DisplayName").Value;
                rdr.ImagePath = exportE.Attribute("ImagePath").Value;
                Items.Add(rdr);
            }
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            XElement rootE = new XElement("FDResourceConfiguration");
            rootE.Add(new XAttribute("FDIdentification", FDIdentification));
            foreach (FDResource item in Items)
            {
                XElement itemE = new XElement("Item");
                itemE.Add(new XAttribute("Id", item.Identifier));
                itemE.Add(new XAttribute("DisplayName", item.DisplayName));
                itemE.Add(new XAttribute("ImagePath", item.ImagePath));
                rootE.Add(itemE);
            }

            doc.Add(rootE);
            return doc.ToString();
        }

        #endregion
    }
}
