using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using OSECoreUI.Graphics;

namespace OSEUI.WPF.Graphics
{
    public class ImageFormatValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ImageFormat format)
            {
                return TypeDescriptor.GetConverter(typeof(ImageFormat)).ConvertToString(format);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s)
            {
                return TypeDescriptor.GetConverter(typeof(ImageFormat)).ConvertFrom(s);
            }

            return null;
        }

        public string[] Strings => GetStrings();

        public static string[] GetStrings()
        {
            List<string> list = new List<string>();
            TypeConverter tc = TypeDescriptor.GetConverter(typeof(ImageFormat));
            foreach (ImageFormat f in Enum.GetValues(typeof(ImageFormat)))
            {
                list.Add(tc.ConvertToString(f));
            }

            return list.ToArray();
        }
    }
}