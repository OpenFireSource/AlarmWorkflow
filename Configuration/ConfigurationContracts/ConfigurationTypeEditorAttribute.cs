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

namespace AlarmWorkflow.Windows.ConfigurationContracts
{
    /// <summary>
    /// Provides detailed information about a type that implements <see cref="ITypeEditor"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ConfigurationTypeEditorAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the type that this type editor can edit.
        /// </summary>
        public Type SourceType { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigurationTypeEditorAttribute"/> class from being created.
        /// </summary>
        private ConfigurationTypeEditorAttribute()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationTypeEditorAttribute"/> class.
        /// </summary>
        /// <param name="sourceType">The type that this type editor can edit.</param>
        public ConfigurationTypeEditorAttribute(Type sourceType)
            : this()
        {
            SourceType = sourceType;
        }

        #endregion
    }
}