using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OSEUI.WPF.Graphics
{
    public interface IPointSeries : IList<Point>, IComparer<Point>, IEqualityComparer<Point>, IFormattable
    {
        double Tolerance { get; }
        Point Head { get; }
        Point Tail { get; }
        Point[] ToArray();
        IPointSeries SubSet(Rect box);
        IPointSeries EndPoints { get; }
    }
}
