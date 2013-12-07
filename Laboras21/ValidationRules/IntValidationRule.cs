using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Laboras21.ValidationRules
{
    class IntValidationRule : ValidationRule
    {
        public IntValidationRule()
        {
        }

        public int Min { get; set; }
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

            if (intValue >= Min && intValue <= Max)
            {
                return new ValidationResult(true, null);
            }
            else
            {
                return new ValidationResult(false, "Value has to be in bounds [" + Min + "; " + Max + "].");
            }
        }
    }
}
