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
using System.Windows;

namespace AlarmWorkflow.Windows.ConfigurationContracts
{
    /// <summary>
    /// Defines a means for an editor that can edit an object of a given type.
    /// </summary>
    public interface ITypeEditor
    {
        /// <summary>
        /// Gets/sets the value that is edited.
        /// </summary>
        /// <exception cref="T:ValueException">Thrown when fetching the value from this editor,
        /// and the user-entered value is not a valid value for the type this editor represents.</exception>
        object Value { get; set; }
        /// <summary>
        /// Gets the visual element that is editing the value.
        /// </summary>
        UIElement Visual { get; }

        /// <summary>
        /// Initializes this TypeEditor-instance.
        /// </summary>
        /// <param name="editorParameter">An optional editor parameter.</param>
        void Initialize(string editorParameter);
    }
}