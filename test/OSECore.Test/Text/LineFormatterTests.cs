using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSECore.Text;

namespace OSECore.Test.Text
{
    public class LineFormatterTests
    {
        [Fact]
        public void CtorTest()
        {
            var lm0 = new LineMargins("(2,60,20)");
            var ts0 = new TabStops("(4,10,30)");
            var d0 = new LineFormatter();
            var d1 = new LineFormatter(lm0);
            var d2 = new LineFormatter(lm0, ts0);
            Assert.Equal(new LineMargins(), d0.Margins);
            Assert.Equal(new TabStops(), d1.TabStops);
            Assert.Equal(lm0, d2.Margins);
            Assert.Equal(ts0, d2.TabStops);
        }
        [Theory]
        [InlineData("(2,20,4)","(6,10,12)","abc",new string[] { " abc" })]
        [InlineData("(2,20,4)", "(6,10,12)", "abc\t\tdef", new string[] { " abc     def" })]
        [InlineData("(2,20,4)", "(6,10,12)", "abc def ghi klm nop qrs", 
            new string[] { " abc def ghi klm","   nop qrs" })]
        [InlineData("(2,20,4)", "(6,10,12)", "abc\tdef\tghi klm nop qrs",
            new string[] { " abc def ghi klm", "         nop qrs" })]
        public void FormatLineTest(string lm0, string ts0, string s0, string[] s1)
        {
            var d = new LineFormatter(new LineMargins(lm0), new TabStops(ts0));
            var r1 = d.FormatLine(s0);
            Assert.Equal(s1, r1);
        }
    }
}
