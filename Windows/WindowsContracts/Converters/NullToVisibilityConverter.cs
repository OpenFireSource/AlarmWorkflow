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