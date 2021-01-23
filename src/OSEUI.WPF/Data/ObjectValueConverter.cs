using OSECore.Object;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace OSEUI.WPF.Data
{
    public class ObjectValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && targetType == typeof(string))
            {
                ObjectConverter dc = ObjectConverters.Find(value.GetType());
                if (dc != null)
                {
                    try
                    {
                        return dc.Format(value);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Unable to format " + value.ToString() + ": " + e.Message);
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObjectConverter dc = ObjectConverters.Find(targetType);
            if (dc != null && value is string s)
            {
                try
                {
                    return dc.Construct(s);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Unable to parse " + value.ToString() + ": " + e.Message);

                }
            }

            return null;

        }
    }
}
