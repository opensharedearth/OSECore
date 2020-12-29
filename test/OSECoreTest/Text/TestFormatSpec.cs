using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public class TestFormatSpec
    {
        [Theory]
        [InlineData("{0}",0,3,0,0,"",true)]
        [InlineData(" {1,-10:F2} ",1,10,1,-10,"F2",true)]
        [InlineData("{0F0}",0,5,0,0,"",false)]
        [InlineData("0F0", 0, 3, 0, 0, "", false)]
        [InlineData("{0:}", 0, 4, 0, 0, "", false)]
        public void TestCtor(string s0, int i0, int l0, int x0, int a0, string f0, bool v0)
        {
            FormatSpec fs = new FormatSpec(s0,i0,l0);
            Assert.Equal(x0, fs.Index);
            Assert.Equal(a0, fs.Alignment);
            Assert.Equal(f0, fs.Format);
            Assert.Equal(v0, fs.IsValid);
        }
        [Theory]
        [InlineData("{0}",0,3,"123",0,typeof(int),3,123)]
        [InlineData("{0:F2}",0,6,"12345.6abc",0,typeof(double),7,12345.6)]
        [InlineData("{0:F2}", 0, 6, "(12345.6abc)", 1, typeof(double), 8, 12345.6)]
        [InlineData("{0:X}",0,5, "FF",0,typeof(int), 2, 255)]
        [InlineData("{0:X}", 0, 5, "FFFFFFFF", 0, typeof(int), 8, -1)]
        public void TestParse(string s0, int i0, int l0, string t0, int k0, Type y0, int j0, object d0)
        {
            FormatSpec fs = new FormatSpec(s0, i0, l0);
            int j1 = fs.Parse(t0, k0, y0, out object d1);
            Assert.Equal(j0, j1);
            Assert.Equal(d0, d1);
        }
        [Theory]
        [InlineData("", -1, -12, -23, -100)]
        [InlineData("c", -1, -12, -23, -100)]
        [InlineData("g", -1, -12, -23, -100)]
        [InlineData("G", -1, -12, -23, -100)]
        public void TestParseTimeSpan(string f0, int d0, int h0, int m0, int ms0)
        {
            string format = f0.Length > 0 ? "{0:" + f0 + "}" : "{0}";
            FormatSpec fs = new FormatSpec(format, 0, format.Length);
            TimeSpan ts0 = new TimeSpan(d0, h0, m0, ms0);
            String l0 = String.Format("aaa" + format + "bbb", ts0);
            int j1 = fs.Parse(l0, 3, typeof(TimeSpan), out object d1);
            Assert.Equal(l0.Length - 3, j1);
            Assert.Equal(d1, ts0);
        }
        [Theory]
        [InlineData("",2018,2,7,2,57,53)]
        [InlineData("d", 2018, 2, 7, 0, 0, 0)]
        [InlineData("D", 2018, 2, 7, 0, 0, 0)]
        [InlineData("f", 2018, 2, 7, 2, 57, 0)]
        [InlineData("F", 2018, 2, 7, 2, 57, 53)]
        [InlineData("g", 2018, 2, 7, 2, 57, 0)]
        [InlineData("M", 0, 2, 7, 0, 0, 0)]
        [InlineData("O", 2018, 2, 7, 2, 57, 53)]
        [InlineData("R", 2018, 2, 7, 2, 57, 53)]
        [InlineData("s", 2018, 2, 7, 2, 57, 53)]
        [InlineData("t", 0, 0, 0, 2, 57, 0)]
        [InlineData("T", 0, 0, 0, 2, 57, 53)]
        [InlineData("u", 2018, 2, 7, 2, 57, 53)]
        [InlineData("U", 2018, 2, 7, 2, 57, 53)]
        [InlineData("Y", 2018, 2, 1, 0, 0, 0)]
        public void TestParseDateTime(string f0, int y0, int m0, int d0, int h0, int mn0, int s0)
        {
            string format = f0.Length > 0 ? "{0:" + f0 + "}" : "{0}";
            FormatSpec fs = new FormatSpec(format, 0, format.Length);
            DateTime dtnow = DateTime.Now;
            if (y0 == 0) y0 = dtnow.Year;
            if (m0 == 0) m0 = dtnow.Month;
            if (d0 == 0) d0 = dtnow.Day;
            DateTime dt00 = new DateTime(y0, m0, d0, h0, mn0, s0);
            DateTime dt0 = DateTime.SpecifyKind(dt00, DateTimeKind.Utc);
            String l0 = String.Format("***" + format + "***", dt0);
            int j1 = fs.Parse(l0, 3, typeof(DateTime), out object d1);
            Assert.Equal(l0.Length - 3, j1);
            Assert.Equal(d1, dt0);
        }
    }
}
