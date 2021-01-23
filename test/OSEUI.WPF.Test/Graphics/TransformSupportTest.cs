using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xunit;
using System.Windows.Media;
using OSECoreUI.Graphics;
using OSEUI.WPF.Graphics;
using Range = OSECoreUI.Graphics.Range;

namespace OSEUI.WPF.Test.Graphics
{
    public class TransformSupportTest
    {
        [Theory]
        [InlineData(Orientation.Horizontal, "(0,100)", "(0,1)", "100,10", new string[] { "0,0", "100,1" }, new string[] { "0,10", "100,0" })]
        [InlineData(Orientation.Horizontal, "(-500,500)", "(-0.5,0.5)", "100,10", new string[] { "0,0", "500,0.5" }, new string[] { "50,5", "100,0" })]
        [InlineData(Orientation.Horizontal, "(100,0)", "(1,0)", "100,10", new string[] { "0,0", "100,1" }, new string[] { "100,0", "0,10" })]
        [InlineData(Orientation.Horizontal, "(500,-500)", "(0.5,-0.5)", "100,10", new string[] { "0,0", "500,0.5" }, new string[] { "50,5", "0,10" })]
        [InlineData(Orientation.Horizontal, "(100,0)", "(0,1)", "100,10", new string[] { "0,0", "100,1" }, new string[] { "100,10", "0,0" })]
        [InlineData(Orientation.Horizontal, "(500,-500)", "(-0.5,0.5)", "100,10", new string[] { "0,0", "500,0.5" }, new string[] { "50,5", "0,0" })]
        [InlineData(Orientation.Horizontal, "(0,100)", "(1,0)", "100,10", new string[] { "0,0", "100,1" }, new string[] { "0,0", "100,10" })]
        [InlineData(Orientation.Horizontal, "(-500,500)", "(0.5,-0.5)", "100,10", new string[] { "0,0", "500,0.5" }, new string[] { "50,5", "100,10" })]
        [InlineData(Orientation.Vertical, "(0,100)", "(0,1)", "10,100", new string[] { "0,0", "100,1" }, new string[] { "0,100", "10,0" })]
        [InlineData(Orientation.Vertical, "(-500,500)", "(-0.5,0.5)", "10,100", new string[] { "0,0", "500,0.5" }, new string[] { "5,50", "10,0" })]
        [InlineData(Orientation.Vertical, "(100,0)", "(1,0)", "10,100", new string[] { "0,0", "100,1" }, new string[] { "10,0", "0,100" })]
        [InlineData(Orientation.Vertical, "(500,-500)", "(0.5,-0.5)", "10,100", new string[] { "0,0", "500,0.5" }, new string[] { "5,50", "0,100" })]
        [InlineData(Orientation.Vertical, "(100,0)", "(0,1)", "10,100", new string[] { "0,0", "100,1" }, new string[] { "0,0", "10,100" })]
        [InlineData(Orientation.Vertical, "(500,-500)", "(-0.5,0.5)", "10,100", new string[] { "0,0", "500,0.5" }, new string[] { "5,50", "10,100" })]
        [InlineData(Orientation.Vertical, "(0,100)", "(1,0)", "10,100", new string[] { "0,0", "100,1" }, new string[] { "10,100", "0,0" })]
        [InlineData(Orientation.Vertical, "(-500,500)", "(0.5,-0.5)", "10,100", new string[] { "0,0", "500,0.5" }, new string[] { "5,50", "0,0" })]
        public void GetTransformTest(Orientation orientation, string xrange, string yrange, string size, string[] pp0, string[] pp1)
        {
            Range xr0 = Range.Parse(xrange);
            Range yr0 = Range.Parse(yrange);
            Size s0 = Size.Parse(size);
            Transform t = TransformSupport.GetTransform(orientation, xr0, yr0, s0);
            for (int i = 0; i < pp0.Length; ++i)
            {
                Point p0 = Point.Parse(pp0[i]);
                Point p1 = Point.Parse(pp1[i]);
                Point p = t.Transform(p0);
                Assert.Equal(p1.X, p.X, 5);
                Assert.Equal(p1.Y, p.Y, 5);
            }
        }
    }
}
