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

namespace AlarmWorkflow.BackendService.SettingsContracts
{
    /// <summary>
    /// Represents an exception that is thrown when a setting identifier was not found.
    /// </summary>
    [Serializable()]
    public class SettingIdentifierNotFoundException : Exception
    {
        #region Properties

        /// <summary>
        /// Gets the name of the identifier that was not found.
        /// </summary>
        public string IdentifierName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="SettingIdentifierNotFoundException"/> class from being created.
        /// </summary>
        private SettingIdentifierNotFoundException()
            : base()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingIdentifierNotFoundException"/> class.
        /// </summary>
        /// <param name="identifierName">The name of the identifier that was not found.</param>
        public SettingIdentifierNotFoundException(string identifierName)
            : base(string.Format(Properties.Resources.SettingIdentifierNotFoundExceptionMessage, identifierName))
        {
            this.IdentifierName = identifierName;
        }

        #endregion
    }
}