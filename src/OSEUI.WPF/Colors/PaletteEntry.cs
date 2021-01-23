using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OSEUI.WPF.Colors
{
    public struct PaletteEntry
    {
        public PaletteEntry(int index = 0, Color color = new Color())
        {
            Index = index;
            Color = color;
        }
        public int Index;
        public Color Color;
        public override string ToString()
        {
            return $"({Index},{Color})";
        }
    }
}
