using OSECoreUI.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OSEUI.WPF.Colors
{
    public static class ColorSupport
    {
        public static Func<Color, float> ChannelGetter(ColorChannel channel, ColorSpace space = ColorSpace.sRGB)
        {
            if (space == ColorSpace.sRGB)
            {
                switch (channel)
                {
                    default:
                    case ColorChannel.Blue: return (c) => c.B / 255.0f;
                    case ColorChannel.Green: return (c) => c.G / 255.0f;
                    case ColorChannel.Red: return (c) => c.R / 255.0f;
                }
            }
            else
            {
                switch (channel)
                {
                    default:
                    case ColorChannel.Blue: return (c) => c.ScB;
                    case ColorChannel.Green: return (c) => c.ScG;
                    case ColorChannel.Red: return (c) => c.ScR;
                }
            }
        }
        public static Func<Color, float, Color> ChannelSetter(ColorChannel channel, ColorSpace space = ColorSpace.sRGB)
        {
            if (space == ColorSpace.sRGB)
            {
                switch (channel)
                {
                    default:
                    case ColorChannel.Blue:
                        return (c, v) =>
                        {
                            c.B = Convert.ToByte(v * 255.0f);
                            return c;
                        };
                    case ColorChannel.Green:
                        return (c, v) =>
                        {
                            c.G = Convert.ToByte(v * 255.0f);
                            return c;
                        };
                    case ColorChannel.Red:
                        return (c, v) =>
                        {
                            c.R = Convert.ToByte(v * 255.0f);
                            return c;
                        };
                }

            }
            else
            {
                switch (channel)
                {
                    default:
                    case ColorChannel.Blue:
                        return (c, v) =>
                        {
                            c.ScB = v;
                            return c;
                        };
                    case ColorChannel.Green:
                        return (c, v) =>
                        {
                            c.ScG = v;
                            return c;
                        };
                    case ColorChannel.Red:
                        return (c, v) =>
                        {
                            c.ScR = v;
                            return c;
                        };
                }

            }
        }

        public static Color FromArgb(int v)
        {
            byte r = (byte)((v & 0xff0000) >> 16);
            byte g = (byte)((v & 0xff00) >> 8);
            byte b = (byte)(v & 0xff);
            return Color.FromRgb(r, g, b);
        }
    }
}
