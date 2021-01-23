using System.ComponentModel;

namespace OSECoreUI.Graphics
{
    [TypeConverter(typeof(ImageFormatTypeConverter))]
    public enum ImageFormat
    {
        [Description("Portable Network Graphics Format")]
        PNG,
        [Description("Joint Photographic Experts Group Format")]
        JPG,
        [Description("Graphics Interchange Format")]
        GIF,
        [Description("Windows Bitmap")]
        BMP,
        [Description("Tagged Image Format")]
        TIFF,
        [Description("Windows Media Photo Format")]
        WDP
    }
}