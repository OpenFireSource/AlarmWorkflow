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

using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace AlarmWorkflow.Windows.Configuration.AddressBookEditor.Validation
{
    /// <summary>
    /// Validation rule for e-mail addresses.
    /// </summary>
    public class MailAddressValidationRule : ValidationRule
    {
        #region Fields

        private static readonly Regex EmailRegex = new Regex(@"^[\w\.=-]+@[\w\.-]+\.[\w]{2,4}$");

        #endregion

        #region Methods

        /// <summary>
        /// Validates that the value is a valid e-mail address.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            string address = value as string;
            if (string.IsNullOrWhiteSpace(address))
            {
                return new ValidationResult(false, Properties.Resources.VR_NoValue);
            }

            return new ValidationResult(EmailRegex.IsMatch(address), Properties.Resources.VR_Mail_InvalidAddress);
        }

        #endregion
    }
}