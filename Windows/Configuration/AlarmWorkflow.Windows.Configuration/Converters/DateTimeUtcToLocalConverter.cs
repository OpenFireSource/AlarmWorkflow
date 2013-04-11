using System;
using System.Windows.Data;

namespace AlarmWorkflow.Windows.Configuration.Converters
{
    class DateTimeUtcToLocalConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dateTime = (DateTime)value;
            return dateTime.ToLocalTime();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
