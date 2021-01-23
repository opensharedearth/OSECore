using OSEUI.WPF.Colors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OSECoreUI.Colors;
using OSEUI.WPF.Graphics;
using Xunit;
using ColorPalette = OSEUI.WPF.Colors.ColorPalette;

namespace OSEUI.WPF.Test.Colors
{
    public class ColorPaletteTests : IClassFixture<ColorFixture>
    {
        private ColorFixture _fixture;

        public ColorPaletteTests(ColorFixture fixture)
        {
            _fixture = fixture;
        }
        [Fact]
        public void TestCtor()
        {
            ColorPalette ut = new ColorPalette();
            Assert.Equal(256, ut.Colors.Length);
        }
        [Fact]
        public void Test1ArgCtor()
        {
            ColorPalette ut = new ColorPalette(_fixture.GreyColors);
            Assert.Equal(256, ut.Colors.Length);
            Assert.Equal(Color.FromRgb(0, 0, 0), ut.Colors[0]);
            Assert.Equal(Color.FromRgb(255, 255, 255), ut.Colors[255]);
        }
        [Fact]
        public void TestGetSetChannel()
        {
            ColorPalette ut = new ColorPalette();
            float[] scale = new float[256];
            float ds = 1.0f / 255.0f;
            for (int i = 0; i < 256; ++i)
            {
                scale[i] = i * ds;
            }

            ut.SetChannel(ColorChannel.Blue, scale);
            ut.SetChannel(ColorChannel.Green, scale);
            ut.SetChannel(ColorChannel.Red, scale);
            Assert.Equal(_fixture.GreyColors, ut.Colors);
            float[] red1 = ut.GetChannel(ColorChannel.Red);
            float[] blue1 = ut.GetChannel(ColorChannel.Blue);
            float[] green1 = ut.GetChannel(ColorChannel.Green);
            Assert.Equal(256, red1.Length);
            Assert.Equal(256, blue1.Length);
            Assert.Equal(256, green1.Length);
            for (int i = 0; i < 256; ++i)
            {
                Assert.Equal(scale[i], red1[i], 5);
                Assert.Equal(scale[i], blue1[i], 5);
                Assert.Equal(scale[i], green1[i], 5);
            }

            ut.Space = ColorSpace.scRGB;
            ut.SetChannel(ColorChannel.Blue, scale);
            ut.SetChannel(ColorChannel.Green, scale);
            ut.SetChannel(ColorChannel.Red, scale);
            for (int i = 0; i < 256; ++i)
            {
                Assert.Equal(_fixture.ScGreyColors[i].ScB, ut.Colors[i].ScB, 5);
                Assert.Equal(_fixture.ScGreyColors[i].ScG, ut.Colors[i].ScG, 5);
                Assert.Equal(_fixture.ScGreyColors[i].ScR, ut.Colors[i].ScR, 5);
            }
            float[] scred1 = ut.GetChannel(ColorChannel.Red);
            float[] scblue1 = ut.GetChannel(ColorChannel.Blue);
            float[] scgreen1 = ut.GetChannel(ColorChannel.Green);
            Assert.Equal(256, scred1.Length);
            Assert.Equal(256, scblue1.Length);
            Assert.Equal(256, scgreen1.Length);
            for (int i = 0; i < 256; ++i)
            {
                Assert.Equal(scale[i], scred1[i], 5);
                Assert.Equal(scale[i], scblue1[i], 5);
                Assert.Equal(scale[i], scgreen1[i], 5);
            }
        }
        [Fact]
        public void TestGetSetControlPoints()
        {
            ColorPalette ut = new ColorPalette(_fixture.GreyColors);
            IPointSeries p = ut.GetControlPoints(ColorChannel.Blue);
            Assert.NotNull(p);
            Assert.Equal(2, p.Count);
            PointSeries ps = new PointSeries(p);
            ps.Add(new Point(128.0, 0.0));
            ut.UpdateControlPoints(ColorChannel.Blue, ps);
            Assert.Equal(0, ut.Colors[100].B);
            Assert.Equal(255, ut.Colors[255].B);
            IPointSeries p1 = ut.GetControlPoints(ColorChannel.Blue, 5e-3f);
            Assert.Equal(3, p1.Count);
        }
    }
}
