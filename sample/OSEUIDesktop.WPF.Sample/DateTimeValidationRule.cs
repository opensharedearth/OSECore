using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OSEUIDesktop.WPF.Sample
{
    class DateTimeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(!DateTime.TryParse(value as string, out DateTime dt))
            {
                return new ValidationResult(false, "Invalid date time string");
            }
            return new ValidationResult(true, null);
        }
    }
}
