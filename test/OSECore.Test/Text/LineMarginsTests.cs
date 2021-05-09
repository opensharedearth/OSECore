using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSECore.Text;

namespace OSECore.Test.Text
{
    public class LineMarginsTests
    {
        [Fact]
        public void CtorTest()
        {
            int l0 = 5;
            int r0 = 100;
            int hi0 = 20;
            var d0 = new LineMargins(l0, r0, hi0);
            Assert.Equal(l0, d0.LeftMargin);
            Assert.Equal(r0, d0.RightMargin);
            Assert.Equal(hi0, d0.HangingIndent);
            Assert.ThrowsAny<Exception>(() => new LineMargins(-1, 100, 10));
            Assert.ThrowsAny<Exception>(() => new LineMargins(1, 1, 10));
            Assert.ThrowsAny<Exception>(() => new LineMargins(1, 100, 100));
        }
        [Fact]
        public void CtorStringTest()
        {
            string s0 = "(2,40,20)";
            string s1 = "(2,40)";
            var d = new LineMargins(s0);
            Assert.Equal(s0, d.ToString());
            Assert.Throws<ArgumentException>(() => new LineMargins(s1));
        }
        [Fact]
        public void GetLeftPaddingTest()
        {
            var d0 = new LineMargins();
            string s1 = d0.GetLeftPadding();
            Assert.Equal(d0.LeftMargin, s1.Length + 1);
        }
        [Fact]
        public void GetHangingIndentPadding()
        {
            var d0 = new LineMargins();
            string s1 = d0.GetHangingIndentPadding();
            Assert.Equal(d0.HangingIndent - 1, s1.Length);
        }
        [Fact]
        public void EqualsTest()
        {
            var lm0 = new LineMargins("(2,20,10)");
            var lm1 = new LineMargins("(2,20,11)");
            var lm2 = new LineMargins("(2,20,10)");
            LineMargins lm3 = null;
            Assert.True(lm0 == lm2);
            Assert.True(lm0 != lm1);
            Assert.False(lm0 == lm3);
            Assert.False(lm3 == lm0);
            Assert.True(lm0 != lm3);
            Assert.True(lm3 != lm0);
        }
    }
}
