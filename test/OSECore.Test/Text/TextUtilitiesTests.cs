using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using OSECore.Text;
using Xunit;

namespace OSECoreTest.Text
{
    public class TextUtilitiesTests : IClassFixture<TextFileFixture>
    {
        private TextFileFixture _fixture;

        public TextUtilitiesTests(TextFileFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("abc ;cde", ';', false, "abc ;cde")]
        [InlineData("abc ;cde", ';', true, "abc")]
        [InlineData(";cde", ';', true, "")]
        [InlineData("   ;cde", ';', true, "")]
        [InlineData("   ;cde", ';', false, "")]
        [InlineData("abc", ';', true, "abc")]
        [InlineData("    abc", ';', false, "    abc")]
        [InlineData("", ';', false, "")]
        public void RemoveCommentTest(string l0, char comment, bool allowInline, string l1)
        {
            string l = TextSupport.RemoveComment(l0, comment, allowInline);
            Assert.Equal(l1, l);
        }

        [Theory]
        [InlineData("&amp;", "&")]
        [InlineData("aaa&amp;aaa", "aaa&aaa")]
        [InlineData("aaa&xxx;", "aaa&xxx;")]
        [InlineData("aaa", "aaa")]
        public void EvaluateCharacterEntitiesTest(string l0, string l1)
        {
            string l = TextSupport.EvaluateCharacterEntities(l0);
            Assert.Equal(l1, l);
        }

        [Theory]
        [InlineData("<a>", "aaa", "<a>aaa</a>\r\n")]
        [InlineData("<a", "aaa", "aaa")]
        public void PutTaggedBlockTest(string tag, string l0, string l1)
        {
            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                l1 = l1.Replace("\r\n", Environment.NewLine);
            }
            string l = TextSupport.PutTaggedBlock(tag, l0, 1);
            Assert.Equal(l1, l);
        }

        [Theory]
        [InlineData("abc", 1, 8, "abc\r\n")]
        [InlineData("abc", 8, 1, "abc")]
        [InlineData("abc", 2, 8, " abc\r\n")]
        [InlineData("", 2, 8, "")]
        [InlineData("aaa aaa aaa", 2, 10, " aaa aaa\r\n aaa\r\n")]
        public void GetWrappedTextTest(string s0, int scol, int ecol, string s1)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                s1 = s1.Replace("\r\n", Environment.NewLine);
            }
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                s1 = s1.Replace("\r\n", Environment.NewLine);
            }
            string s = TextSupport.GetWrappedText(s0, scol, ecol);
            Assert.Equal(s1, s);
        }

        [Theory]
        [InlineData("<a>aaa</a>", "aaa", false, false)]
        [InlineData("<a>aaa", "", false, false)]
        [InlineData("<a>aaa", "aaa aaa</b>", true, false)]
        [InlineData("<b>aaa", "aaa aaa", true, false)]
        [InlineData("", "", false, false)]
        [InlineData("<c>", "", true, true)]
        public void GetTaggedBlockTest(string l0, string l1, bool useFile, bool throws)
        {
            if (useFile)
            {
                using (StreamReader r = File.OpenText(_fixture.TaggedBlockFilePath))
                {
                    if (throws)
                    {
                        Assert.Throws<ApplicationException>(() => TextSupport.GetTaggedBlock(l0, r));
                    }
                    else
                    {
                        string l = TextSupport.GetTaggedBlock(l0, r);
                        Assert.Equal(l1, l);
                    }
                }

            }
            else
            {
                string l = TextSupport.GetTaggedBlock(l0);
                Assert.Equal(l1, l);
            }
        }

        [Fact]
        public void PutCharEntitiesTest()
        {
            string s0 = "<a>";
            string s1 = "&lt;a&gt;";
            string s = TextSupport.PutCharacterEntities(s0);
            Assert.Equal(s1, s);
        }

        [Theory]
        [InlineData("aaa", '\'', TextSupport.QuoteOptions.None, "'aaa'")]
        [InlineData("aaa'aaa", '\'', TextSupport.QuoteOptions.Double, "'aaa''aaa'")]
        [InlineData("aaa'", '\'', TextSupport.QuoteOptions.Double, "'aaa'''")]
        [InlineData("aaa'aaa", '\'', TextSupport.QuoteOptions.Escape, "'aaa\\'aaa'")]
        public void QuoteTest(string s0, char q, TextSupport.QuoteOptions q0, string s1)
        {
            string s = TextSupport.Quote(s0, q, q0);
            Assert.Equal(s1, s);
        }

        [Theory]
        [InlineData("'aaa'", '\'', TextSupport.QuoteOptions.None, "aaa")]
        [InlineData("'aaa''aaa'", '\'', TextSupport.QuoteOptions.Double, "aaa'aaa")]
        [InlineData("'aaa'''", '\'', TextSupport.QuoteOptions.Double, "aaa'")]
        [InlineData("'aaa\\'aaa'", '\'', TextSupport.QuoteOptions.Escape, "aaa'aaa")]
        [InlineData("'aaa'aaa'", '\'', TextSupport.QuoteOptions.Escape, "aaa'aaa")]
        [InlineData("'aaa'aaa'", '\'', TextSupport.QuoteOptions.Double, "aaa'aaa")]
        public void UnquoteTest(string s0, char q, TextSupport.QuoteOptions q0, string s1)
        {
            string s = TextSupport.Unquote(s0, q, q0);
            Assert.Equal(s1, s);
        }

        [Theory]
        [InlineData("    ", 4, "\t")]
        [InlineData("\t  a", 2, "\t\ta")]
        [InlineData("    a b", 2, "\t\ta b")]
        [InlineData("", 2, "")]
        [InlineData("aa", 2, "aa")]
        public void EntabTest(string s0, int tabSize, string s1)
        {
            string s = TextSupport.Entab(s0, tabSize);
            Assert.Equal(s1, s);
        }

        [Theory]
        [InlineData("\t", 4, "    ")]
        [InlineData("\t\ta\tb", 2, "    a b")]
        [InlineData("", 2, "")]
        [InlineData("aa", 2, "aa")]
        public void DetabTest(string s0, int tabSize, string s1)
        {
            string s = TextSupport.Detab(s0, tabSize);
            Assert.Equal(s1, s);
        }

        [Theory]
        [InlineData("aaa", 0, 4, 1)]
        [InlineData("\t\taaa", 3, 4, 10)]
        [InlineData("aaa\t", 3, 4, 4)]
        public void GetColumnFromOffsetTest(string l0, int o0, int tabSize, int c1)
        {
            int c = TextSupport.GetColumnFromOffset(l0, o0, tabSize);
            Assert.Equal(c1, c);
        }

        [Theory]
        [InlineData("aaa", 1, 4, 0)]
        [InlineData("\t\taaa", 10, 4, 3)]
        [InlineData("aaa\t", 4, 4, 3)]
        public void GetOffsetFromColumnTest(string l0, int c0, int tabSize, int o1)
        {
            int c = TextSupport.GetOffsetFromColumn(l0, c0, tabSize);
            Assert.Equal(o1, c);
        }

        private const string hexChars = "0123456789ABCDEFabcdef";

        private static int[] hexValues =
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 10, 11, 12, 13, 14, 15
        };

        [Fact]
        public void GetHexValueText()
        {
            for (int i = 0; i < hexChars.Length; ++i)
            {
                int v1 = TextSupport.GetHexValue(hexChars[i]);
                Assert.Equal(v1, hexValues[i]);
            }

            Assert.Equal(0, TextSupport.GetHexValue('Q'));
        }
        [Fact]
        public void PutHexCharText()
        {
            for (int i = 0; i < 16; ++i)
            {
                char c1 = TextSupport.PutHexChar(i);
                Assert.Equal(c1, hexChars[i]);
            }

            Assert.Equal('\0', TextSupport.PutHexChar(100));
        }

        private const string characterEntities =
            "\"&'<> ¡¢£¤¥¦§¨©ª«¬­®"
            + "¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂ"
            + "ÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ"
            + "×ØÙÚÛÜÝÞßàáâãäåæçèéê"
            + "ëìíîïðñòóôõö÷øùúûüýþ"
            + "ÿŒœŠšŸƒˆ˜ΑΒΓΔΕΖΗΘΙΚΛ"
            + "ΜΝΞΟΠΡΣΤΥΦΧΨΩαβγδεζη"
            + "θικλμνξοπρςστυφχψωϑϒ"
            + "ϖ   ‌‍‎‏–—‘’‚“”„†‡•…"
            + "‰′″‹›‾⁄€ℑ℘ℜ™ℵ←↑→↓↔↵⇐"
            + "⇑⇒⇓⇔∀∂∃∅∇∈∉∋∏∑−∗√∝∞∠"
            + "∧∨∩∪∫∴∼≅≈≠≡≤≥⊂⊃⊄⊆⊇⊕⊗"
            + "⊥⋅⌈⌉⌊⌋〈〉◊♠♣♥♦";
        [Fact]
        public void CharacterEntitiesTest()
        {
            for (int i = 0; i < characterEntities.Length; ++i)
            {
                char c0 = characterEntities[i];
                Assert.True(TextSupport.IsCharacterEntity(c0),String.Format("{0} ({1}) at index {2} is not a character entity.", c0, Convert.ToInt32(c0), i));
                string s0 = TextSupport.PutCharacterEntity(c0);
                Assert.NotEmpty(s0);
                char c1 = TextSupport.GetCharacterEntity(s0);
                Assert.Equal(c0, c1);
            }
        }
    }
}
