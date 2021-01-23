using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace OSECoreUI.Colors
{
    public class ColorConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(int) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is int d)
            {
                return GetColor(d);
            }
            else if (value is string s)
            {
                return GetColor(s);
            }

            return Color.Empty;
        }

        public static Color GetColor(int argb)
        {
            int argb1 = (int)((argb & 0x00ffffff) | 0xff000000);
            return Color.FromArgb(argb1);
        }

        public static Color GetColor(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (name[0] == '#' && int.TryParse(name.Substring(1), NumberStyles.HexNumber, null, out int v))
                {
                    return GetColor(v);
                }
                else
                {
                    return Color.FromName(name);
                }
            }

            return Color.Empty;
        }

        public static string ToString(Color c)
        {
            if (c.IsNamedColor)
            {
                return c.Name;
            }
            else
            {
                return $"#{(c.ToArgb()&0xffffff):x}";
            }
        }
    }
}
