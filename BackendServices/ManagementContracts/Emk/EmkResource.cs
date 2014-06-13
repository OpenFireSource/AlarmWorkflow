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
using System.Runtime.Serialization;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.BackendService.ManagementContracts.Emk
{
    /// <summary>
    /// Represents a single resource of a fire department.
    /// EMK is an abbreviation for "EinsatzMittelKonfiguration".
    /// </summary>
    [DataContract()]
    public sealed class EmkResource : IEquatable<EmkResource>
    {
        #region Properties

        /// <summary>
        /// Gets/sets the site abbreviation that the underlying fire department is named in the alarm source.
        /// </summary>
        [DataMember()]
        public string SiteAlias { get; set; }
        /// <summary>
        /// Gets/sets the name or identifier of the vehicle etc. that is used to identify it in the alarm source.
        /// </summary>
        [DataMember()]
        public string ResourceAlias { get; set; }
        /// <summary>
        /// Gets/sets the name of this resource to use when displaying it.
        /// </summary>
        [DataMember()]
        public string DisplayName { get; set; }
        /// <summary>
        /// Gets/sets the image path to a server-side icon file.
        /// </summary>
        [DataMember()]
        public string IconFileName { get; set; }
        /// <summary>
        /// Gets/sets whether or not this resource shall be taken into consideration when executing a query.
        /// </summary>
        [DataMember()]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets the generated identifier for this resource.
        /// The identifier is a concatenation of <see cref="SiteAlias"/> and <see cref="ResourceAlias"/>.
        /// </summary>
        public string Id
        {
            get { return string.Format("{0}.{1}", SiteAlias, ResourceAlias); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmkResource"/> class.
        /// </summary>
        public EmkResource()
        {
            this.IsActive = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether or not this and the given <see cref="OperationResource"/> instance can be considered equal.
        /// </summary>
        /// <param name="other">The other instance to check.</param>
        /// <returns></returns>
        public bool IsMatch(OperationResource other)
        {
            if (other == null || string.IsNullOrWhiteSpace(other.FullName))
            {
                return false;
            }

            return other.FullName.Contains(SiteAlias)
                && other.FullName.Contains(ResourceAlias);
        }

        #endregion

        #region IEquatable<EmkResource> Members

        /// <summary>
        /// Determines whether or not this and another instance are considerd equal.
        /// </summary>
        /// <param name="other">The other instance to check.</param>
        /// <returns></returns>
        public bool Equals(EmkResource other)
        {
            if (other == null)
            {
                return false;
            }

            return this.SiteAlias == other.SiteAlias
                && this.ResourceAlias == other.ResourceAlias;
        }

        #endregion
    }
}
