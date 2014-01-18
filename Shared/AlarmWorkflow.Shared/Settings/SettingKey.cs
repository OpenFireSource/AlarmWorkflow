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
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Shared.Settings
{
    /// <summary>
    /// Identifies a single setting by its identifier and setting name.
    /// </summary>
    [Serializable()]
    public sealed class SettingKey : IEquatable<SettingKey>
    {
        #region Constants

        private const string FullNameFormat = "{0}.{1}";

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the setting.
        /// </summary>
        public string Identifier { get; private set; }
        /// <summary>
        /// Gets the name of the setting.
        /// </summary>
        public string Name { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingKey"/> class from being created.
        /// </summary>
        private SettingKey()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new <see cref="SettingKey"/> instance.
        /// </summary>
        /// <param name="identifier">The identifier of the setting.</param>
        /// <param name="name">The name of the setting.</param>
        /// <returns>A new <see cref="SettingKey"/> instance.</returns>
        public static SettingKey Create(string identifier, string name)
        {
            Assertions.AssertNotEmpty(identifier, "identifier");
            Assertions.AssertNotEmpty(name, "name");

            return new SettingKey() { Identifier = identifier, Name = name };
        }

        /// <summary>
        /// Returns a string that represents this instance of <see cref="SettingKey"/>.
        /// </summary>
        /// <returns>A string that is similar to "Identifier.Name".</returns>
        public override string ToString()
        {
            return string.Format(FullNameFormat, Identifier, Name);
        }

        #endregion

        #region IEquatable<SettingKey> Members

        /// <summary>
        /// Returns whether or not the given <see cref="SettingKey"/> instance is equal to the current instance.
        /// </summary>
        /// <param name="other">The other instance to check equality to this instance.</param>
        /// <returns>Whether or not the given <see cref="SettingKey"/> instance is equal to the current instance.</returns>
        public bool Equals(SettingKey other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Identifier.Equals(other.Identifier)
                && this.Name.Equals(other.Name);
        }

        #endregion
    }
}
