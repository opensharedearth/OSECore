using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OSECore.Text;

namespace OSECore.Test.Text
{
    public class TextFormatterTests
    {
        [Fact]
        public void FormatCollectionTest()
        {
            int[] i0 = new int[] { 1, 3, 5 };
            int[] i1 = new int[] { };
            int[] i2 = null;
            string s0 = "(1,3,5)";
            string s1 = "()";
            string s2 = "()";
            Assert.Equal(s0, TextFormatter.FormatCollection(i0));
            Assert.Equal(s1, TextFormatter.FormatCollection(i1));
            Assert.Equal(s2, TextFormatter.FormatCollection(i2));
        }
    }
}
