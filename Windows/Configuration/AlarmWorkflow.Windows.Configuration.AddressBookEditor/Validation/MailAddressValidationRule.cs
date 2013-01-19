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
