using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace OSEUIDesktop.WPF.Sample
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is DateTime dt)
            {
                return dt.ToString(App.Instance.Settings.DateFormatString);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string s)
            {
                if(DateTime.TryParse(s, out DateTime dt))
                {
                    return dt;
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
