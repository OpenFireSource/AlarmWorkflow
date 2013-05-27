using System;
using System.Windows.Data;

namespace AlarmWorkflow.Windows.UIContracts.Converters
{
    /// <summary>
    /// Custom <see cref="IValueConverter"/> that converts a <see cref="DateTime"/> from UTC to local time.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(DateTime))]
    public sealed class DateTimeUtcToLocalConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            return dateTime.ToLocalTime();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion

    }
}
