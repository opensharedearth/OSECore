using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using OSECore.Logging;

namespace OSEUI.WPF.Converters
{
    public class ResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Result r)
            {
                if (targetType == typeof(string))
                {
                    return r.ToString();
                }
                else if (targetType == typeof(Color))
                {
                    return GetColor(r.Type);
                }
                else if (targetType == typeof(Brush))
                {
                    return GetBrush(r.Type);
                }
                else if (targetType == typeof(ImageSource))
                {
                    return GetIconPath(r.Type);
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        public static Color GetColor(ResultType rt)
        {
            switch (rt)
            {
                case ResultType.Bad:
                    return System.Windows.Media.Colors.DarkRed;
                case ResultType.Good:
                    return System.Windows.Media.Colors.DarkGreen;
                case ResultType.Suspect:
                    return System.Windows.Media.Colors.DarkOrange;
                case ResultType.Unknown:
                default:
                    return System.Windows.Media.Colors.Black;
            }
        }
        public static Brush GetBrush(ResultType rt)
        {
            switch (rt)
            {
                case ResultType.Bad:
                    return Brushes.DarkRed;
                case ResultType.Good:
                    return Brushes.DarkGreen;
                case ResultType.Suspect:
                    return Brushes.DarkOrange;
                case ResultType.Unknown:
                default:
                    return Brushes.Black;
            }
        }
        public static string GetIconPath(ResultType rt)
        {
            switch (rt)
            {
                case ResultType.Bad:
                    return "pack://application:,,,/OSEUIForms.WPF;component/Images/badresult.png";
                case ResultType.Good:
                    return "pack://application:,,,/OSEUIForms.WPF;component/Images/goodresult.png";
                case ResultType.Suspect:
                    return "pack://application:,,,/OSEUIForms.WPF;component/Images/suspectresult.png";
                case ResultType.Unknown:
                    return "pack://application:,,,/OSEUIForms.WPF;component/Images/unknownresult.png";
                default:
                    return null;
            }
        }
    }
}
