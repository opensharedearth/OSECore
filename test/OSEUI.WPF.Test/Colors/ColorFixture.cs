using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OSEUI.WPF.Test.Colors
{
    public class ColorFixture : IDisposable
    {
        public Color[] RedColors = new Color[256];
        public Color[] GreenColors = new Color[256];
        public Color[] BlueColors = new Color[256];
        public Color[] GreyColors = new Color[256];
        public Color[] ScRedColors = new Color[256];
        public Color[] ScGreenColors = new Color[256];
        public Color[] ScBlueColors = new Color[256];
        public Color[] ScGreyColors = new Color[256];

        public ColorFixture()
        {
            for (int i = 0; i < 256; ++i)
            {
                byte b = Convert.ToByte(i);
                float sc = i / 255.0f;
                RedColors[i] = Color.FromRgb(b, 0, 0);
                GreenColors[i] = Color.FromRgb(0, b, 0);
                BlueColors[i] = Color.FromRgb(0, 0, b);
                GreyColors[i] = Color.FromRgb(b, b, b);
                ScRedColors[i] = Color.FromScRgb(1.0f, sc, 0.0f, 0.0f);
                ScGreenColors[i] = Color.FromScRgb(1.0f,0, sc, 0);
                ScBlueColors[i] = Color.FromScRgb(1.0f,0, 0, sc);
                ScGreyColors[i] = Color.FromScRgb(1.0f, sc, sc, sc);
            }
        }

        public void Dispose()
        {
        }
    }
}
