using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OSEUI.WPF.Graphics;
using Xunit;

namespace OSEUI.WPF.Test.Graphics
{
    public class GraphicsAreaTests
    {
        [Fact]
        public void BoundaryCtorTest()
        {
            Rect r = new Rect(0, 0, 10, 100);
            GraphicsArea ut = new GraphicsArea(r);
            Assert.Equal(r, ut.Boundary);
            Assert.Equal(r, ut.Free);
        }

        [Fact]
        public void SizeCtorTest()
        {
            Rect r1 = new Rect(0, 0, 10, 100);
            Size s0 = new Size(10, 100);
            GraphicsArea ut = new GraphicsArea(s0);
            Assert.Equal(r1, ut.Boundary);
            Assert.Equal(r1, ut.Free);
        }

        [Theory]
        [InlineData("0, 0, 10, 100", new string[] {"5,5,5,5"}, new bool[] { true }, "0,10,10,90")]
        [InlineData("0, 0, 10, 100", new string[] { "5,85,5,5" }, new bool[] { true }, "0,0,10,85")]
        [InlineData("0, 0, 100, 10", new string[] { "5,5,5,5" }, new bool[] { true }, "10,0,90,10")]
        [InlineData("0, 0, 100, 10", new string[] { "5,5,5,5","90,0,10,10" }, new bool[] { true, true }, "10,0,80,10")]
        [InlineData("0, 0, 100, 10", new string[] { "5,5,5,5", "90,0,10,10","0,0,10,10","50,0,20,10","10,0,30,10","80,0,10,10", "80,0,30,10" }, new bool[] { true, true, false, true, true, true, false }, "40,0,40,10")]
        public void AllocateTest(string b0, string[] rr0, bool[] a1, string f1)
        {
            GraphicsArea ut = new GraphicsArea(Rect.Parse(b0));
            Assert.Equal(rr0.Length,a1.Length);
            for (int i = 0; i < rr0.Length; ++i)
            {
                Assert.Equal(a1[i], ut.Allocate(Rect.Parse(rr0[i])));
            }

            Assert.Equal(Rect.Parse(f1), ut.Free);
        }
    }
}
