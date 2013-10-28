using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Laboras21
{
    class PositiveIntValidationRule : ValidationRule
    {
        public PositiveIntValidationRule()
        {
        }

        public int Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int intValue;

            try
            {
                intValue = int.Parse((string)value, cultureInfo);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Entered value is not a valid integer.");
            }

            if (intValue > 0 && intValue <= MagicalNumbers.MaxN)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "Value has to be in bounds [0; " + MagicalNumbers.MaxN + "].");
            }
        }
    }
}
