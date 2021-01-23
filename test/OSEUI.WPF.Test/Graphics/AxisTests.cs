using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSEUI.WPF.Graphics;
using Xunit;

namespace OSEUI.WPF.Test.Graphics
{
    public class AxisTests
    {
        [Fact]
        public void TestCtor()
        {
            double min0 = 0.0;
            double max0 = 100.0;
            int ticks0 = 21;
            Axis ut = new Axis(min0, max0, ticks0);
            Assert.Equal(min0, ut.Min);
            Assert.Equal(max0, ut.Max);
            Assert.Equal(ticks0, ut.DesiredTicks);
        }

        [Fact]
        public void TestProperties()
        {
            double min0 = 0.0;
            double max0 = 100.0;
            int ticks0 = 20;
            Axis ut = new Axis(min0, max0, ticks0);
            ut.Min = -max0;
            Assert.Equal(2 * max0, ut.Range);
            ut.DesiredTicks = 2;
            Assert.Equal(2, ut.ActualTicks);
            Assert.Equal(2 * max0, ut.TickIncrement);
            ut.Max = min0;
            Assert.Equal(max0, ut.Range, 5);
        }

        [Theory]
        [InlineData(0.0, 100.0, 11, 11, 10.0, new double[] { 0.0, 10.0, 20.0, 30.0, 40.0, 50.0, 60.0, 70.0, 80.0, 90.0,
            100.0
    })]
        [InlineData(0.0,1.0,3,3,0.5,new double[] { 0.0, 0.5, 1.0 })]

    public void TestGetTicks(double min0, double max0, int ticks0, int ticks1, double inc1, double[] labels1)
        {
            Axis ut = new Axis(min0, max0, ticks0);
            Assert.Equal(ticks1, ut.ActualTicks);
            Assert.Equal(inc1, ut.TickIncrement, 5);
            double[] labels = ut.GetTicks();
            Assert.Equal(labels1.Length, labels.Length);
            for (int i = 0; i < labels1.Length; ++i)
            {
                Assert.Equal(labels1[i], labels1[i],5);
            }

        }
    }
}
