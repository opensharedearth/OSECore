using System;
using System.Xml.XPath;
using Xunit;
using OSECore;
using OSECore.Logging;

namespace OSECoreTest.Logging
{
    public class ResultTests
    {
        [Theory]
        [InlineData(ResultType.Unknown, "1", "1")]
        [InlineData(ResultType.Good, "2", "2")]
        [InlineData(ResultType.Bad, "3", "Error: 3")]
        [InlineData(ResultType.Suspect, "4", "Warning: 4")]
        public void Ctor(ResultType t0, string d0, string s0)
        {
            Result r = new Result(t0, d0);
            string s1 = r.ToString();
            Assert.Equal(s0, s1);
        }

        [Fact]
        public void TypeTest()
        {
            Result r = new Result(ResultType.Bad, "Bad result");
            ResultType t = r.Type;
            Assert.Equal(ResultType.Bad, t);
        }
    }
}
