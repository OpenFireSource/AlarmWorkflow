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
