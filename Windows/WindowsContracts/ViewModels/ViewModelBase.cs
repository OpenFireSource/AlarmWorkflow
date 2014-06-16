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

using System.ComponentModel;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UIContracts.ViewModels
{
    /// <summary>
    /// ViewModel base class.
    /// </summary>
    public abstract class ViewModelBase : DisposableObject, INotifyPropertyChanged
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ViewModelBase class.
        /// </summary>
        public ViewModelBase()
        {
            CommandHelper.WireupRelayCommands(this);
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when the value of a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Manually raises the PropertyChanged event for the given property.
        /// </summary>
        /// <param name="propertyName">The name of the property to raise this event for.</param>
        public virtual void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Called when this viewmodel shall perform cleanup work prior to it being disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">Thrown if the model was already disposed.</exception>
        protected override void DisposeCore()
        {
            CommandHelper.UnwireRelayCommands(this);
        }

        #endregion
    }
}