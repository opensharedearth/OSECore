using System;
using System.Collections.Generic;
using System.Text;
using OSECore;
using OSECore.Text;
using Xunit;

namespace OSECoreTest
{
    public class UnicodeSupportTests
    {
        private static string normal = "1234567890-+";
        private static string superscripts = "¹²³⁴⁵⁶⁷⁸⁹⁰⁻⁺";
        private static string subscripts = "₁₂₃₄₅₆₇₈₉₀₋₊";
        private static string altminus = "\x2212\x207b\x208b";
        private static string altplus = "\x207a\x208a";
        private static string altslash = "\x2215";
        private static string altstar = "\x22c5";

        [Fact]
        public void GetNormalCharTest()
        {
            for(int i = 0; i < normal.Length; i++)
                Assert.Equal(UnicodeSupport.GetNormalChar(superscripts[i]),normal[i]);
            for (int i = 0; i < normal.Length; i++)
                Assert.Equal(UnicodeSupport.GetNormalChar(subscripts[i]), normal[i]);
            for (int i = 0; i < altminus.Length; i++)
                Assert.Equal('-', UnicodeSupport.GetNormalChar(altminus[i]));
            for (int i = 0; i < altplus.Length; i++)
                Assert.Equal('+', UnicodeSupport.GetNormalChar(altplus[i]));
            for (int i = 0; i < altslash.Length; i++)
                Assert.Equal('/', UnicodeSupport.GetNormalChar(altslash[i]));
            for (int i = 0; i < altstar.Length; i++)
                Assert.Equal('*', UnicodeSupport.GetNormalChar(altstar[i]));
        }

        [Fact]
        public void GetNormalTextTest()
        {
            string t0 = UnicodeSupport.GetNormalText(superscripts);
            string t1 = UnicodeSupport.GetNormalText(subscripts);
            string t2 = UnicodeSupport.GetNormalText(altminus);
            string t3 = UnicodeSupport.GetNormalText(altplus);
            string t4 = UnicodeSupport.GetNormalText(altslash);
            string t5 = UnicodeSupport.GetNormalText(altstar);
            Assert.Equal(t0, normal);
            Assert.Equal(t1, normal);
            Assert.Equal(t2, new string('-', altminus.Length));
            Assert.Equal(t3, new string('+', altplus.Length));
            Assert.Equal(t4, new string('/', altslash.Length));
            Assert.Equal(t5, new string('*', altstar.Length));
        }

        [Fact]
        public void IsSuperscriptCharTest()
        {
            foreach(char c in normal)
                Assert.False(UnicodeSupport.IsSuperscriptChar(c));
            foreach (char c in subscripts)
                Assert.False(UnicodeSupport.IsSuperscriptChar(c));
            foreach (char c in superscripts)
                Assert.True(UnicodeSupport.IsSuperscriptChar(c));
        }
        [Fact]
        public void GetSuperscriptCharTest()
        {
            for (int i = 0; i < normal.Length; ++i)
                Assert.Equal(superscripts[i], UnicodeSupport.GetSuperscriptChar(normal[i]));
        }
        [Fact]
        public void GetSuperscriptTextTest()
        {
            Assert.Equal(superscripts,UnicodeSupport.GetSuperscriptText(normal));
        }
        [Fact]
        public void IsSubscriptCharTest()
        {
            foreach (char c in normal)
                Assert.False(UnicodeSupport.IsSubscriptChar(c));
            foreach (char c in subscripts)
                Assert.True(UnicodeSupport.IsSubscriptChar(c));
            foreach (char c in superscripts)
                Assert.False(UnicodeSupport.IsSubscriptChar(c));

        }
        [Fact]
        public void GetSubscriptCharTest()
        {
            for (int i = 0; i < normal.Length; ++i)
                Assert.Equal(subscripts[i], UnicodeSupport.GetSubscriptChar(normal[i]));
        }
        [Fact]
        public void GetSubscriptTextTest()
        {
            Assert.Equal(subscripts, UnicodeSupport.GetSubscriptText(normal));
        }
    }
}
