using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Laboras21.ValidationRules
{
    class CoordinateValidationRule : ValidationRule
    {
        public CoordinateValidationRule()
        {
        }
        
        private int min;
        private int max;
        public string Dimension 
        {
            set
            {
                if (value == "x")
                {
                    min = MagicalNumbers.MinX;
                    max = MagicalNumbers.MaxX;
                }
                else if (value == "y")
                {
                    min = MagicalNumbers.MinY;
                    max = MagicalNumbers.MaxY;
                }
                else
                {
                    throw new ArgumentException(@"Dimension has to be either ""x"" or ""y""!");
                }
            }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int coordinate;

            try
            {
                coordinate = int.Parse((string)value);
            }
            catch (Exception)
            {
                return new ValidationResult(false, "Entered value is not an integer.");
            }

            if (coordinate < min || coordinate > max)
            {
                return new ValidationResult(false, "Coordinate has to be in bounds [" + min + "; " + max + "].");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
