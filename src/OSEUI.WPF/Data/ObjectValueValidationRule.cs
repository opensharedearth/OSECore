using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using OSECore.Object;

namespace OSEUI.WPF.Data
{
    public class ObjectValueValidationRule : ValidationRule
    {
        private Type _valueType;

        public Type ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (_valueType != null)
            {
                ObjectConverter oc = ObjectConverters.Instance[_valueType];
                if (oc != null && value is string s)
                {
                    object[] dl = oc.Parse(s);
                    if(oc.Validate(dl))
                        return ValidationResult.ValidResult;
                    return new ValidationResult(false, oc.GetDescription());
                }
            }

            return new ValidationResult(false, "No valid conversion found.");
        }
    }
}
