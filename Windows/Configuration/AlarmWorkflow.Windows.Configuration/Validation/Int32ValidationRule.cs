using System.Windows.Controls;

namespace AlarmWorkflow.Windows.Configuration.Validation
{
    /// <summary>
    /// Represents a validation rule that enforces Int32-boundaries.
    /// </summary>
    public class Int32ValidationRule : ValidationRule
    {
        /// <summary>
        /// Enforces Int32-validation rules.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value != null)
            {
                string sv = (string)value;

                int i = 0;
                if (int.TryParse(sv, out i))
                {
                    return ValidationResult.ValidResult;
                }
            }

            return new ValidationResult(false, string.Format("Zahl muss innerhalb von '{0}' und '{1}' sein!", int.MinValue, int.MaxValue));
        }
    }
}
