using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace OSECoreUI.Graphics
{
    public class ImageFormatTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                int i = s.IndexOf(':');
                if (i > 0) s = s.Substring(0, i);
                switch (s)
                {
                    case "BMP": return ImageFormat.BMP;
                    case "GIF": return ImageFormat.GIF;
                    case "JPG":
                    case "JPEG":
                        return ImageFormat.JPG;
                    case "PNG": return ImageFormat.PNG;
                    case "TIFF":
                    case "TIF":
                        return ImageFormat.TIFF;
                    case "WDP": return ImageFormat.WDP;
                }
            }

            return null;
        }

        public static string GetDescription(ImageFormat format)
        {
            return format.GetType().GetMember(format.ToString())[0].GetCustomAttribute<DescriptionAttribute>()
                .Description;
        }

        public static string GetFileFilter(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.BMP:
                    return "Windows Bitmap (*.BMP)|*.BMP";
                case ImageFormat.GIF:
                    return "Graphics Interchange Format(*.GIF)|*.GIF";
                case ImageFormat.JPG:
                    return "Joint Photographic Experts Group Format (*.JPG,*.JPEG)|*.JPG;*.JPEG";
                case ImageFormat.PNG:
                    return "Portable Network Graphics Format (*.PNG)|*.PNG";
                case ImageFormat.TIFF:
                    return "Tagged Image Format (*.TIF,*.TIFF)|*.TIF;*.TIFF";
                case ImageFormat.WDP:
                    return "Windows Media Photo Format (*.WDP)|*.WDP";
                default:
                    return "";
            }

        }

        public static string GetFileFilters(bool allFiles, params ImageFormat[] formats)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ImageFormat format in formats)
            {
                if (sb.Length > 0) sb.Append('|');
                sb.Append(GetFileFilter(format));
            }

            if (allFiles)
            {
                if (sb.Length > 0) sb.Append('|');
                sb.Append("All Files (*.*)|*.*");
            }

            return sb.ToString();
        }

        public static bool SupportsMetadata(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.GIF:
                case ImageFormat.JPG:
                case ImageFormat.TIFF:
                    return true;
                default:
                    return false;

            }
        }
        public static string GetDefaultExtension(ImageFormat format)
        {
            switch (format)
            {
                case ImageFormat.BMP:
                    return ".bmp";
                case ImageFormat.GIF:
                    return ".gif";
                case ImageFormat.JPG:
                    return ".jpg";
                case ImageFormat.PNG:
                    return ".png";
                case ImageFormat.TIFF:
                    return ".tiff";
                case ImageFormat.WDP:
                    return ".wdp";
                default: return "";
            }
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is ImageFormat format)
            {
                return format.ToString() + ": " + GetDescription(format);
            }

            return null;
        }
    }
}