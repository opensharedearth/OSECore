using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Converters;
using OSECoreUI.Graphics;

namespace OSEUI.WPF.Graphics
{
    [Serializable]
    [TypeConverter(typeof(PointSeriesConverter))]
    public class PointSeries : IPointSeries, ISerializable
    {
        public const double DefaultTolerance = 1e-3;
        private readonly double _tolerance;
        public PointSeries(double tolerance = DefaultTolerance)
        {
            _tolerance = tolerance;
            _listCore = new List<Point>();
        }
        public PointSeries(IEnumerable<Point> pp0, double tolerance = DefaultTolerance)
        {
            _tolerance = tolerance;
            _listCore = new List<Point>(pp0);
        }

        public PointSeries(PointSeries ps0)
        {
            _tolerance = ps0._tolerance;
            _listCore = new List<Point>(ps0._listCore);
        }

        public PointSeries(SerializationInfo info, StreamingContext context)
        {
            try
            {
                _tolerance = info.GetDouble("Tolerance");
                _listCore = new List<Point>();
                int n = info.GetInt32("Count");
                for (int i = 0; i < n; ++i)
                {
                    double x = info.GetDouble("X" + i);
                    double y = info.GetDouble("Y" + i);
                    _listCore.Add(new Point(x, y));
                }

            }
            catch (Exception e)
            {
                throw new SerializationException("Unable to deserialize PointList", e);
            }

        }

        private List<Point> _listCore;

        public IEnumerator<Point> GetEnumerator()
        {
            return _listCore.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_listCore).GetEnumerator();
        }

        public void Add(Point item)
        {
            int index = _listCore.BinarySearch(item, this);
            if(index < 0)
                _listCore.Insert(~index, item);
            else
            {
                _listCore.Insert(index, item);
            }
        }

        public void Clear()
        {
            _listCore.Clear();
        }

        public bool Contains(Point item)
        {
            return _listCore.Contains(item, this);
        }

        public void CopyTo(Point[] array, int arrayIndex)
        {
            _listCore.CopyTo(array, arrayIndex);
        }

        public bool Remove(Point item)
        {
            return _listCore.Remove(item);
        }

        public int Count => _listCore.Count;

        public bool IsReadOnly => false;

        public int IndexOf(Point item)
        {
            int index = _listCore.BinarySearch(item, this);
            return index >= 0 ? index : -1;
        }

        public void Insert(int index, Point item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            if(_listCore.Count > 0)_listCore.RemoveAt(Clamp(index));
        }

        public Point this[int index]
        {
            get => _listCore.Count > 0 ? _listCore[Clamp(index)] : new Point();
            set
            {
                if (_listCore.Count > 0)
                {
                    _listCore[Clamp(index)] = value;
                }
            }
        }

        public Point Head => this[0];
        public Point Tail => this[Count - 1];
        public IPointSeries EndPoints => new PointSeries(new Point[] { Head, Tail }, _tolerance);

        public double Tolerance => _tolerance;

        private int Clamp(int index)
        {
            return Math.Max(0, Math.Min(_listCore.Count - 1, index));
        }

        public Point[] ToArray() => _listCore.ToArray();

        public IPointSeries SubSet(Rect box)
        {
            PointSeries selected = new PointSeries();
            foreach (Point p in this)
            {
                if (box.Contains(p))
                {
                    selected.Add(p);
                }
            }

            return selected;
        }

        public static IPointSeries Difference(IPointSeries a, IPointSeries b, double tolerance = DefaultTolerance)
        {
            if (a != null && b != null)
            {
                PointSeries c = new PointSeries(tolerance);
                for (int i = 0, j = 0; i < a.Count;)
                {
                    if (j < b.Count)
                    {
                        if (Math.Abs(a[i].X - b[j].X) < tolerance)
                        {
                            i++;
                            j++;
                        }
                        else if (a[i].X < b[j].X)
                        {
                            c.Add(a[i]);
                            i++;
                        }
                        else
                        {
                            j++;
                        }
                    }
                    else
                    {
                        c.Add(a[i]);
                        i++;
                    }
                }

                return c;

            }

            return a;
        }

        public static IPointSeries Union(params IPointSeries[] pss)
        {
            if (pss.Length > 2)
            {
                PointSeries c = new PointSeries(pss[0]);
                foreach (IPointSeries a in pss)
                {
                    for (int i = 0, j = 0; i < a.Count;)
                    {
                        if (j < c.Count)
                        {
                            if (Math.Abs(a[i].X - c[j].X) < c.Tolerance)
                            {
                                i++;
                                j++;
                            }
                            else if (a[i].X < c[j].X)
                            {
                                c.Add(a[i]);
                                i++;
                            }
                            else
                            {
                                j++;
                            }
                        }
                        else
                        {
                            c.Add(a[i]);
                            i++;
                        }
                    }
                }

                return c;
            }
            else if (pss.Length == 1)
            {
                return pss[0];
            }
            else
            {
                return new PointSeries();
            }
        }

        public static IPointSeries Intersection(IPointSeries a, IPointSeries b, double tolerance = DefaultTolerance)
        {
            if (a != null && b != null)
            {
                PointSeries c = new PointSeries(tolerance);
                for (int i = 0, j = 0; i < a.Count && j < b.Count;)
                {
                    if (Math.Abs(a[i].X - b[j].X) < tolerance)
                    {
                        c.Add(a[i]);
                        i++;
                        j++;
                    }
                    else if (a[i].X < b[j].X)
                    {
                        i++;
                    }
                    else
                    {
                        j++;
                    }
                }

                return c;
            }

            return new PointSeries();
        }
        int IComparer<Point>.Compare(Point x, Point y)
        {
            double dx = x.X - y.X;
            if (Math.Abs(dx) <= _tolerance)
                return 0;
            else if (dx < 0)
                return -1;
            else
                return 1;
        }

        bool IEqualityComparer<Point>.Equals(Point x, Point y)
        {
            return Math.Abs(x.X - y.X) <= _tolerance;
        }

        int IEqualityComparer<Point>.GetHashCode(Point obj)
        {
            return obj.X.GetHashCode();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Tolerance", _tolerance);
            info.AddValue("Count", Count);
            for (int i = 0; i < Count; ++i)
            {
                Point p = this[i];
                info.AddValue("X" + i, p.X);
                info.AddValue("Y" + i, p.Y);
            }
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            PointSeriesConverter psc = new PointSeriesConverter();
            return psc.ConvertToString(this);
        }

        public override string ToString()
        {
            return ToString("", null);
        }
    }
}
