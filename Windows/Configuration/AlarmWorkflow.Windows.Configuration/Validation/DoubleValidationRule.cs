using System.Globalization;
using System.Windows.Controls;

namespace AlarmWorkflow.Windows.Configuration.Validation
{
    /// <summary>
    /// Represents a validation rule that enforces Double-boundaries.
    /// </summary>
    public class DoubleValidationRule : ValidationRule
    {
        /// <summary>
        /// Enforces Double-validation rules.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {
                string sv = (string)value;

                double d = 0;
                if (double.TryParse(sv, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                {
                    return ValidationResult.ValidResult;
                }
            }

            return new ValidationResult(false, string.Format("Zahl muss innerhalb von '{0}' und '{1}' sein!", double.MinValue, double.MaxValue));
        }
    }
}
