using System;
using System.ComponentModel;
using System.Globalization;

namespace OSECore.Logging
{
    public class ResultTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(ResultType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                switch (s)
                {
                    case "Warning": return ResultType.Suspect;
                    case "Error": return ResultType.Bad;
                    default:
                        return ResultType.Good;
                }
            }

            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is ResultType rt)
            {
                switch (rt)
                {
                    case ResultType.Bad: return "Error";
                    case ResultType.Suspect: return "Warning";
                    default: return "";
                }
            }

            return null;
        }

    }
}