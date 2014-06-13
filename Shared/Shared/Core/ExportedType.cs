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
    /// Represents one type that had an <see cref="ExportAttribute"/> applied to it.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("Type = {Type}")]
    public struct ExportedType
    {
        #region Properties

        /// <summary>
        /// Gets the attribute that specifies the export.
        /// </summary>
        public ExportAttribute Attribute { get; private set; }
        /// <summary>
        /// Gets the type that is exported.
        /// </summary>
        public Type Type { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ExportedType struct.
        /// </summary>
        /// <param name="attribute">The attribute that specifies the export.</param>
        /// <param name="type">The type that is exported.</param>
        public ExportedType(ExportAttribute attribute, Type type)
            : this()
        {
            this.Attribute = attribute;
            this.Type = type;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of this type.
        /// </summary>
        /// <typeparam name="T">The expected type.</typeparam>
        /// <returns>An instance of this type.</returns>
        public T CreateInstance<T>()
        {
            return (T)Activator.CreateInstance(this.Type);
        }

        #endregion

    }
}