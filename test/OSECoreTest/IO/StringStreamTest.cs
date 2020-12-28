using System;
using System.IO;
using OSECore.IO;
using Xunit;

namespace OSECoreTest.IO
{
    public class StringStreamTest
    {
        [Fact]
        public void TestRead()
        {
            string s0 = "This is the test string.";
            string s1 = null;
            StringStream ss = new StringStream(s0);
            using (StreamReader reader = new StreamReader(ss))
            {
                s1 = reader.ReadLine();
            }
            Assert.Equal(s0,s1);
        }

        [Fact]
        public void TestWrite()
        {
            string s0 = "This is the test string.";
            StringStream ss = new StringStream();
            using (StreamWriter writer = new StreamWriter(ss))
            {
                writer.WriteLine(s0);
                writer.Flush();
            }
            string s1 = ss.ToString();
            Assert.Equal(s0 + "\r\n", s1);
        }

        [Fact]
        public void TestReadWrite()
        {
            string s0 = "This is the test string.";
            StringStream ss0 = new StringStream();
            string s1 = null;
            string s2 = null;
            using (StreamWriter writer = new StreamWriter(ss0))
            {
                writer.WriteLine(s0);
                writer.Flush();
            }

            ss0.Position = 0;
            using (StreamReader reader = new StreamReader(ss0))
            {
                s2 = reader.ReadToEnd();
            }
            s1 = ss0.ToString();
            Assert.Equal(s0 + "\r\n", s1);
            Assert.Equal(s0 + "\r\n", s2);
        }

        [Fact]
        public void TestCapacityCtor()
        {
            string s0 = new string('a', 100);
            StringStream ss = new StringStream(50);
            string s1 = null;
            using (StreamWriter writer = new StreamWriter(ss))
            {
                writer.WriteLine(s0);
            }

            s1 = ss.ToString();
            Assert.Equal(s0 + "\r\n", s1);
        }

        [Fact]
        public void TestSeek()
        {
            StringStream ss0 = new StringStream("abcdef");
            ss0.Seek(-2, SeekOrigin.End);
            int b = ss0.ReadByte();
            Assert.Equal('e', b);
        }

        [Fact]
        public void TestPosition()
        {
            StringStream ss0 = new StringStream("abcdef");
            ss0.Position = 4;
            int b = ss0.ReadByte();
            Assert.Equal('e', b);
        }

        [Fact]
        public void TestReadWriteByte()
        {
            StringStream ss1 = new StringStream();
            ss1.WriteByte(Convert.ToByte('a'));
            Assert.Equal(1, ss1.Length);
            ss1.Position = 0;
            Assert.Equal('a', ss1.ReadByte());
        }

        [Fact]
        public void TestSetLength()
        {
            StringStream ss0 = new StringStream();
            ss0.Write(System.Text.Encoding.UTF8.GetBytes("abcdef"), 0, 6);
            ss0.SetLength(3);
            Assert.Equal("abc",ss0.ToString());
        }
    }
}