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

using System.Windows;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.CustomViewer.Extensibility
{
    /// <summary>
    /// Defines a new IUIWidget.
    /// </summary>
    public interface IUIWidget
    {
        /// <summary>
        /// Gets the <see cref="UIElement" /> which is used to represent the widget.
        /// </summary>
        UIElement UIElement { get; }
        /// <summary>
        /// Gets the unique identifier of the widget, which is required for serialization and should be uinque.
        /// </summary>
        string ContentGuid { get; }
        /// <summary>
        /// Gets the title of the widget.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Initializes this widget.
        /// </summary>
        /// <returns>The result of the initialization. Widgets that return false won't be called.</returns>
        bool Initialize();
        /// <summary>
        /// Called when the selected operation has changed and we need to display the given one.
        /// </summary>
        /// <param name="operation">The <see cref="Operation" /> that was selected and is now being displayed.</param>
        void OnOperationChange(Operation operation);
    }
}