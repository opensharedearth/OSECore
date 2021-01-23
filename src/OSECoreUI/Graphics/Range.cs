using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Text;

namespace OSECoreUI.Graphics
{
    public struct Range
    {
        public Order Order;
        public double Max;
        public double Min;

        public Range(double start = 0.0, double end = 0.0)
        : this(end > start ? Order.Ascending : Order.Descending, Math.Min(start,end), Math.Max(start,end))
        {
        }

        public Range(Order order, double min, double max)
        {
            Order = order;
            Min = min;
            Max = max;
        }
        public bool IsValid => Max > Min;
        public double Extent => Max - Min;

        public static Range Parse(string s)
        {
            string format = "({0:g},{1:g})";
            if (!String.IsNullOrEmpty(s))
            {
                object[] args = s.Parse(format, typeof(double), typeof(double));
                if(args.Length == 2)
                {
                    return new Range((double)args[0], (double)args[1]);
                }
            }

            return new Range();
        }
    }
}
