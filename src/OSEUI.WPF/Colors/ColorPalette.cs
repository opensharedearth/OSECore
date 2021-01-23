using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using OSECoreUI.Colors;
using OSEUI.WPF.Graphics;

namespace OSEUI.WPF.Colors
{
    [TypeConverter(typeof(ColorPaletteConverter))]
    public class ColorPalette : IColorPalette, INotifyPropertyChanged
    {
        private const int DefaultSize = 256;
        private Color[] _colors;
        private ColorSpace _colorSpace = ColorSpace.sRGB;

        public ColorPalette()
        {
            SetMonochromePalette(System.Windows.Media.Colors.Black);
        }

        public ColorPalette(Color[] colors)
        {
            _colors = new Color[colors.Length];
            Array.Copy(colors, _colors, colors.Length);
        }

        public ColorPalette(PaletteDefinition definition)
        {
            SetPalette(definition);
        }

        public Color[] Colors => _colors;

        public void SetPalette([NotNull] PaletteDefinition definition)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            SetMonochromePalette(System.Windows.Media.Colors.Black, definition.Size);
            UpdateControlPoints(ColorChannel.Blue, definition.GetControlPoints(ColorChannel.Blue));
            UpdateControlPoints(ColorChannel.Green, definition.GetControlPoints(ColorChannel.Green));
            UpdateControlPoints(ColorChannel.Red, definition.GetControlPoints(ColorChannel.Red));
        }

        public void SetMonochromePalette(Color color, int size = DefaultSize)
        {
            _colors = new Color[size];
            for (int i = 0; i < size; ++i)
            {
                _colors[i] = color;
            }
        }
        public float[] GetChannel(ColorChannel channel)
        {
            float[] d0 = new float[Size];
            for (int i = 0; i < Size; ++i)
            {
                d0[i] = ColorSupport.ChannelGetter(channel, Space)(_colors[i]);
            }

            return d0;
        }

        public void SetChannel(ColorChannel channel, float[] colors, int start = 0, int length = Int32.MaxValue)
        {
            int n = Math.Min(Math.Min(Size - start, length), colors.Length);
            for (int i = 0; i < n; ++i)
            {
                _colors[i + start] = ColorSupport.ChannelSetter(channel, Space)(_colors[i + start], colors[i]);
            }
        }

        public int Size => _colors.Length;

        public Color this[int index]
        {
            get
            {
                int i = Math.Max(0, Math.Min(Size - 1, index));
                return _colors[i];
            }
        }
        public ColorSpace Space
        {
            get => _colorSpace;
            set
            {
                if (_colorSpace != value)
                {
                    _colorSpace = value;
                    OnPropertyChanged(nameof(ColorSpace));
                }
            }
        }


        public IPointSeries GetControlPoints(ColorChannel channel, float tolerance = 1e-3f)
        {
            PointSeries pp = new PointSeries();
            float[] values = GetChannel(channel);
            float[] slopes = new float[values.Length - 1];

            for (int i = 0; i < slopes.Length; ++i)
            {
                slopes[i] = values[i + 1] - values[i];
            }

            pp.Add(new Point(0.0, values[0]));
            for (int i = 0; i < slopes.Length - 1; ++i)
            {
                if (Math.Abs(slopes[i + 1] - slopes[i]) > tolerance)
                {
                    pp.Add(new Point(i + 1, values[i + 1]));
                }
            }
            pp.Add(new Point(Size - 1, values[Size - 1]));
            return pp;
        }

        public void UpdateControlPoints(ColorChannel channel, IPointSeries points)
        {
            int Index(Point p) => (int)Math.Max(0.0, Math.Min((float)Size, p.X));
            float Mag(Point p) => (float)Math.Max(0.0f, Math.Min(1.0f, p.Y));
            Func<Color,float,Color> setter = ColorSupport.ChannelSetter(channel);
            if (points.Count > 1)
            {
                Point p0 = points[0];
                int index0 = Index(p0);
                float mag0 = Mag(p0);

                for (int i = 1; i < points.Count; ++i)
                {
                    Point p1 = points[i];
                    int index1 = Index(p1);
                    float mag1 = Mag(p1);
                    for (int j = index0; j < index1; ++j)
                    {
                        float v = mag0 + (j - index0) * (mag1 - mag0) / (index1 - index0);
                        _colors[j] = setter(_colors[j], v);
                    }

                    index0 = index1;
                    mag0 = mag1;
                }

                _colors[index0] = setter(_colors[index0], mag0);
            }
            OnPropertyChanged(nameof(Colors));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
