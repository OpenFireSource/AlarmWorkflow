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

using System.Windows.Controls;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.Validation
{
    /// <summary>
    /// Validation rule for phone numbers.
    /// </summary>
    public class MobilePhoneValidationRule : ValidationRule
    {
        #region Methods

        /// <summary>
        /// Validates that the value is a valid phone number. This means any string that consists of letters.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string number = value as string;
            if (string.IsNullOrWhiteSpace(number))
            {
                return new ValidationResult(false, Properties.Resources.VR_NoValue);
            }

            foreach (char c in number)
            {
                if (c!= '+' && !char.IsDigit(c))
                {
                    return new ValidationResult(false, Properties.Resources.VR_Phone_InvalidNumber);
                }
            }

            return ValidationResult.ValidResult;
        }

        #endregion
    }
}