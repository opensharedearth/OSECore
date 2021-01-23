using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using OSECoreUI.Colors;
using OSEUI.WPF.Graphics;

namespace OSEUI.WPF.Colors
{
    [TypeConverter(typeof(ColorPaletteConverter))]
    public interface IColorPalette : INotifyPropertyChanged
    {
        Color[] Colors { get; }
        float[] GetChannel(ColorChannel channel);
        void SetChannel(ColorChannel channel, float[] colors, int start = 0, int length = Int32.MaxValue);
        int Size { get; }
        Color this[int index] { get; }
        ColorSpace Space { get; set; }
        IPointSeries GetControlPoints(ColorChannel channel, float tolerance = 1e-3f);
        void UpdateControlPoints(ColorChannel channel, IPointSeries points);
    }
}
