using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSEUI.WPF.Colors
{
    public sealed class ColorPalettes
    {
        private static ColorPalette _greyScale = null;
        private static ColorPalette _blueGreenRed = null;
        private static ColorPalette _rainbow = null;
        public static ColorPalette GreyScale => _greyScale ?? (_greyScale = new ColorPalette(PaletteDefinitions.GrayScale));
        public static ColorPalette BlueGreenRed => _blueGreenRed ?? (_blueGreenRed = new ColorPalette(PaletteDefinitions.BlueGreenRed));
        public static ColorPalette Rainbow => _rainbow ?? (_rainbow = new ColorPalette(PaletteDefinitions.Rainbow));

        public static ColorPalette GetColorPalette(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                switch (name.ToLower())
                {
                    case "greyscale": return GreyScale;
                    case "bluegreenred": return BlueGreenRed;
                    case "rainbow": return Rainbow;
                }
            }

            return null;
        }

        public static string[] PaletteNames => new string[] {"GreyScale", "BlueGreenRed", "Rainbow"};
    }
}
