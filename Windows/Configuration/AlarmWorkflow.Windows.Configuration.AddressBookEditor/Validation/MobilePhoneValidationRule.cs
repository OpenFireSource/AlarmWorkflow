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
                if (!char.IsDigit(c))
                {
                    return new ValidationResult(false, Properties.Resources.VR_Phone_InvalidNumber);
                }
            }

            return ValidationResult.ValidResult;
        }

        #endregion
    }
}
