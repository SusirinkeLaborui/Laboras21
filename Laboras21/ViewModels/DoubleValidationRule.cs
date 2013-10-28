using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Laboras21
{
    class DoubleValidationRule : ValidationRule
    {
        public DoubleValidationRule()
        {
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            double doubleValue;

            try
            {
                doubleValue = double.Parse((string)value, cultureInfo);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Entered value is not a valid number.");
            }

            return new ValidationResult(true, null);
        }
    }
}
