using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OSEUI.WPF.Graphics
{
    public class PointSeriesConverter : TypeConverter
    {
        public const string ClipboardFormat = "OSE.PointSeries";
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(PointSeries);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                PointConverter pc = new PointConverter();
                PointSeries pl = new PointSeries();
                string[] ppl = s.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pp in ppl)
                {
                    pl.Add((Point)pc.ConvertFrom(pp));
                }

                return pl;
            }

            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is PointSeries pl)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Point p in pl)
                {
                    sb.AppendLine(p.ToString());
                }

                return sb.ToString();
            }

            return null;
        }

        public static void SetClipboard(IPointSeries ps)
        {
            try
            {
                Clipboard.Clear();
                DataObject d = new DataObject();
                d.SetData(ClipboardFormat, ps);
                d.SetData(DataFormats.CommaSeparatedValue, ps.ToString());
                Clipboard.SetDataObject(d);
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to set clipboard with Point Series: " + e.Message);
            }
        }

        public static PointSeries GetClipboard()
        {
            try
            {
                if (Clipboard.ContainsData(ClipboardFormat))
                {
                    return Clipboard.GetData(ClipboardFormat) as PointSeries;
                }
                else if (Clipboard.ContainsData(DataFormats.CommaSeparatedValue))
                {
                    PointSeriesConverter psc = new PointSeriesConverter();
                    return psc.ConvertFromString(Clipboard.GetData(DataFormats.CommaSeparatedValue) as string) as PointSeries;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Unable to get Point Series from clipboard: " + e.Message);
            }
            return null;

        }

        public static bool ClipboardDataAvialable()
        {
            return Clipboard.ContainsData(ClipboardFormat) ||
                   Clipboard.ContainsData(DataFormats.CommaSeparatedValue);
        }
    }
}
