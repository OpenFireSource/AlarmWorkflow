using System;
using System.Windows.Data;
using AlarmWorkflow.Shared.Core;

namespace AlarmWorkflow.Windows.UI.Converters
{
    /// <summary>
    /// Provides value conversion from the destination location from <see cref="Operation"/> to a string.
    /// </summary>
    [ValueConversion(typeof(Operation), typeof(string))]
    public sealed class OperationToDestinationStringConverter : IValueConverter
    {
        #region IValueConverter Members

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Operation operation = value as Operation;
            if (operation == null)
            {
                return "";
            }

            return operation.GetDestinationLocation().ToString();
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
