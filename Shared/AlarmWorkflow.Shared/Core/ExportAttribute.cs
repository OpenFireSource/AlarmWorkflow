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
using System.Diagnostics;

namespace AlarmWorkflow.Shared.Core
{
    /// <summary>
    /// Provides export-metadata, such as alias etc.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Alias = {Alias}")]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public sealed class ExportAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the exported alias of this type.
        /// </summary>
        public string Alias { get; private set; }
        /// <summary>
        /// Gets the interface type that this exports.
        /// </summary>
        public Type Type { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAttribute"/> class.
        /// </summary>
        /// <param name="alias">The exported alias of this type.</param>
        /// <param name="exportedType">The type that is exported.</param>
        public ExportAttribute(string alias, Type exportedType)
        {
            Alias = alias;
            Type = exportedType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportAttribute"/> class.
        /// </summary>
        /// <param name="type">The type, used to infer the alias automatically.</param>
        /// <param name="exportedType">Type of the exported.</param>
        public ExportAttribute(Type type, Type exportedType)
            : this(type.FullName, exportedType)
        {
        }

        #endregion
    }
}