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
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using AlarmWorkflow.Shared.Core;
using AlarmWorkflow.Shared.Settings;

namespace AlarmWorkflow.BackendService.ManagementContracts.Emk
{
    /// <summary>
    /// Represents a collection of <see cref="EmkResource"/> instances.
    /// </summary>
    [DataContract()]
    public sealed class EmkResourceCollection : Collection<EmkResource>, IStringSettingConvertible, ICloneable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmkResourceCollection"/> class.
        /// </summary>
        public EmkResourceCollection()
            : base()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Examines every resource that is contained in this collection and returns a boolean value if at least
        /// one resource matches the given <see cref="OperationResource"/>.
        /// See documentation for further information.
        /// </summary>
        /// <remarks>Any given <see cref="EmkResource"/> is only considered if it has its IsActive-flag set to true.</remarks>
        /// <param name="resource">The <see cref="OperationResource"/> to check. Must not be null.</param>
        /// <returns>A boolean value indicating whether or not at least one resource matches the given <see cref="OperationResource"/>.</returns>
        public bool ContainsMatch(OperationResource resource)
        {
            Assertions.AssertNotNull(resource, "resource");

            return Items.Any(item => item.IsActive && item.IsMatch(resource));
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
            foreach (XElement item in doc.Root.Elements("Resource"))
            {
                EmkResource resource = new EmkResource();
                resource.SiteAlias = item.TryGetAttributeValue("SiteAlias", null);
                resource.ResourceAlias = item.TryGetAttributeValue("ResourceAlias", null);
                resource.DisplayName = item.TryGetAttributeValue("DisplayName", null);
                resource.IconFileName = item.TryGetAttributeValue("IconFileName", null);
                resource.IsActive = item.TryGetAttributeValue("IsActive", true);

                this.Add(resource);
            }
        }

        string IStringSettingConvertible.ConvertBack()
        {
            XDocument doc = new XDocument();
            doc.Add(new XElement("Emk"));

            foreach (EmkResource item in this)
            {
                XElement resourceE = new XElement("Resource");
                resourceE.SetAttributeValue("SiteAlias", item.SiteAlias);
                resourceE.SetAttributeValue("ResourceAlias", item.ResourceAlias);
                resourceE.SetAttributeValue("DisplayName", item.DisplayName);
                resourceE.SetAttributeValue("IconFileName", item.IconFileName);
                resourceE.SetAttributeValue("IsActive", item.IsActive);
                doc.Root.Add(resourceE);
            }

            return doc.ToString();
        }

        #endregion

        #region ICloneable Members

        object ICloneable.Clone()
        {
            EmkResourceCollection clone = new EmkResourceCollection();
            clone.AddRange(this.Items);
            return clone;
        }

        #endregion
    }
}
