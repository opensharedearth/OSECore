using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace OSEUI.WPF.Colors
{
    public static class PaletteDefinitions
    {
        public static PaletteDefinition GrayScale = new PaletteDefinition(new Color[]
            {
                System.Windows.Media.Colors.Black,
                System.Windows.Media.Colors.White
            });

        public static PaletteDefinition BlueGreenRed = new PaletteDefinition(new Color[]
            {
                System.Windows.Media.Colors.Blue,
                System.Windows.Media.Colors.Green,
                System.Windows.Media.Colors.Red
            });

        public static PaletteDefinition Rainbow = new PaletteDefinition(new Color[]
            {
                System.Windows.Media.Colors.Red,
                System.Windows.Media.Colors.Orange,
                System.Windows.Media.Colors.Yellow,
                System.Windows.Media.Colors.Green,
                System.Windows.Media.Colors.Blue,
                System.Windows.Media.Colors.Indigo,
                System.Windows.Media.Colors.Violet
            });
    }
}
