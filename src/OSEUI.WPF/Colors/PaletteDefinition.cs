using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OSECore.Text;
using OSECoreUI.Colors;
using OSEUI.WPF.Graphics;

namespace OSEUI.WPF.Colors
{
    public class PaletteDefinition : IList<PaletteEntry>
    {
        private readonly List<PaletteEntry> _list;
        private const int DefaultSize = 256;

        public PaletteDefinition()
        {
            _list = new List<PaletteEntry>();

        }

        public PaletteDefinition(Color[] colors, int size = DefaultSize)
        {
            _list = new List<PaletteEntry>();
            if (colors.Length < 2) throw new ArgumentException("There must be at least 2 colors in palette definition");
            if(colors.Length > size)throw new ArgumentException("Size of palette must be >= length of color array");
            double inc = ((double) size) / (colors.Length - 1);
            for (int i = 0; i < colors.Length; ++i)
            {
                int index = Math.Min(size - 1, (int) (i * inc + 0.5));
                Add(new PaletteEntry(index, colors[i]));
            }
        }

        public int Size => this[Count - 1].Index + 1;

        public PaletteDefinition(IEnumerable<PaletteEntry> entries)
        {
            _list = new List<PaletteEntry>(entries);
            _list.Sort((a, b) => b.Index - a.Index);
            if (Count < 2) throw new ArgumentException("Palette definition must have at least 2 colors.");
            if (this[0].Index != 0) throw new ArgumentException("Palette definition must begin at index 0.");
            if (this[Count - 1].Index > 65535)
                throw new ArgumentException("Palette definition too large, cannot have more than 65536 entries.");
        }

        public PointSeries GetControlPoints(ColorChannel channel, ColorSpace space = ColorSpace.sRGB)
        {
            PointSeries pp = new PointSeries();
            Func<Color, float> getter = ColorSupport.ChannelGetter(channel, space);
            foreach (PaletteEntry entry in this)
            {
                pp.Add(new Point((double) entry.Index, getter(entry.Color)));
            }

            return pp;
        }

        public IEnumerator<PaletteEntry> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _list).GetEnumerator();
        }

        public void Add(PaletteEntry item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(PaletteEntry item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(PaletteEntry[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public bool Remove(PaletteEntry item)
        {
            return _list.Remove(item);
        }

        public int Count => _list.Count;

        public bool IsReadOnly => ((ICollection<PaletteEntry>) _list).IsReadOnly;

        public int IndexOf(PaletteEntry item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, PaletteEntry item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public PaletteEntry this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public static PaletteDefinition GetPaletteDefinition(ColorPalette palette, float tolerance = 1e-3f)
        {
            PaletteDefinition pd = new PaletteDefinition();
            if (palette != null)
            {
                IPointSeries redPS = palette.GetControlPoints(ColorChannel.Red, tolerance);
                IPointSeries greenPS = palette.GetControlPoints(ColorChannel.Green, tolerance);
                IPointSeries bluePS = palette.GetControlPoints(ColorChannel.Blue, tolerance);
                IPointSeries allPS = PointSeries.Union(redPS, greenPS, bluePS);
                foreach (Point p in allPS)
                {
                    int index = (int) Math.Round(p.X);
                    pd.Add(new PaletteEntry(index,palette[index]));
                }
            }

            return pd;
        }

        public override string ToString()
        {
            return TextFormatter.FormatCollection(_list);
        }
    }
}
