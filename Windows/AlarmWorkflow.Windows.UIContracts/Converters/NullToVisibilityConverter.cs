using System;
using System.Windows;
using System.Windows.Data;

namespace AlarmWorkflow.Windows.UIContracts.Converters
{
    /// <summary>
    /// Custom <see cref="IValueConverter"/> that takes any <see cref="Object"/> and interprets a null-value as either <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/>.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        #region Properties

        /// <summary>
        /// Gets/sets which <see cref="Visibility"/> to use for a null-value.
        /// </summary>
        public Visibility NullVisibility { get; set; }
        /// <summary>
        /// Gets/sets which <see cref="Visibility"/> to use for a not-null-value.
        /// </summary>
        public Visibility NotNullVisibility { get; set; }

        #endregion

        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return NullVisibility;
            }
            return NotNullVisibility;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
