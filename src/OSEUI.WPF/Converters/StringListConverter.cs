using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using OSECore.Text;

namespace OSEUI.WPF.Converters
{
    public class StringListConverter : IValueConverter
    {
        public string Delimiter { get; set; }= ";";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string[] slist)
            {
                return String.Join(Delimiter, slist);
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return s.SplitLine(Delimiter);
            }

            return Binding.DoNothing;
        }
    }
}
