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

namespace AlarmWorkflow.Windows.UIContracts.ViewModels
{
    /// <summary>
    /// Provides a checked item that uses a string as the "value" type.
    /// </summary>
    public class CheckedStringItem : CheckedItem<string>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedStringItem"/> class.
        /// </summary>
        /// <param name="value">The initial value to set.</param>
        public CheckedStringItem(string value)
            : base(value)
        {

        }

        #endregion
    }

    /// <summary>
    /// Provides a data class that holds a boolean value (representing "selected" state) and a custom value.
    /// </summary>
    /// <typeparam name="T">The type to use.</typeparam>
    public class CheckedItem<T> : INotifyPropertyChanged
    {
        #region Fields

        private bool _isChecked;
        private T _value;
        private object _tag;

        #endregion

        #region Properties

        /// <summary>
        /// Gets/sets whether this item is checked/selected.
        /// </summary>
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        /// <summary>
        /// Gets/sets the value of this item.
        /// </summary>
        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets/sets an optional tag for information that can be applied to this instance.
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set
            {
                _tag = value;
                OnPropertyChanged("Tag");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedItem{T}"/> class.
        /// </summary>
        public CheckedItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckedItem{T}"/> class.
        /// </summary>
        /// <param name="value">The initial value to set.</param>
        public CheckedItem(T value)
            : this()
        {
            Value = value;
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// See base implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var copy = PropertyChanged;
            if (copy != null)
            {
                copy(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}