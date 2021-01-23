using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using OSECoreUI.Graphics;
using Xunit;
using Range = OSECoreUI.Graphics.Range;

namespace OSECoreUI.Test.Graphics
{

    public class RangeTests
    {
        [Theory]
        [InlineData(Order.Ascending, 0.0, 0.0, false, 0.0)]
        [InlineData(Order.Ascending, 0.0, 100.0, true, 100.0)]
        [InlineData(Order.Ascending, 100.0, 0.0, false, -100.0)]
        [InlineData(Order.Descending, 0.0, 0.0, false, 0.0)]
        [InlineData(Order.Descending, 0.0, 100.0, true, 100.0)]
        [InlineData(Order.Descending, 100.0, 0.0, false, -100.0)]
        public void Ctor3ArgTest(Order order0, double min0, double max0, bool vaiid1, double extent1)
        {
            Range ut0 = new Range(order0, min0, max0);
            Assert.Equal(order0, ut0.Order);
            Assert.Equal(min0, ut0.Min);
            Assert.Equal(max0, ut0.Max);
            Assert.Equal(vaiid1, ut0.IsValid);
            Assert.Equal(extent1, ut0.Extent);
        }
        [Theory]
        [InlineData(0.0, 0.0, false, 0.0, Order.Descending)]
        [InlineData(0.0, 100.0, true, 100.0, Order.Ascending)]
        [InlineData(100.0, 0.0, true, 100.0, Order.Descending)]
        public void Ctor2ArgTest(double min0, double max0, bool vaiid1, double extent1, Order order1)
        {
            Range ut0 = new Range(min0, max0);
            Assert.Equal(Math.Min(min0, max0), ut0.Min);
            Assert.Equal(Math.Max(min0,max0), ut0.Max);
            Assert.Equal(order1, ut0.Order);
            Assert.Equal(vaiid1, ut0.IsValid);
            Assert.Equal(extent1, ut0.Extent);
        }

        [Theory]
        [InlineData("(0.0,100.0)", true, Order.Ascending, 0.0, 100.0)]
        [InlineData("(0.0)", false, Order.Ascending, 0.0, 0.0)]
        public void ParseTest(string s0, bool valid, Order order, double min, double max)
        {
            Range r1 = Range.Parse(s0);
            Assert.Equal(valid, r1.IsValid);
            if (valid)
            {
                Assert.Equal(order, r1.Order);
                Assert.Equal(min, r1.Min);
                Assert.Equal(max, r1.Max);
            }
        }
    }
}
