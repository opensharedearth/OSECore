using OSECore;
using OSECore.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace OSECoreTest.Logging
{
    public class ResultLogTests
    {
        [Fact]
        public void CtorDefaultTest()
        {
            ResultLog l = new ResultLog();
            Assert.Empty(l);
            Assert.Equal(0, l.GoodCount);
            Assert.Equal(0, l.BadCount);
            Assert.Equal(0, l.SuspectCount);
        }

        [Fact]
        public void Ctor2ArgTest()
        {
            ResultLog l0 = new ResultLog();
            l0.LogGood("Good result.");
            l0.LogBad("Error result.");
            ResultLog l1 = new ResultLog();
            l1.LogSuspect("Suspect result.");
            ResultLog l = new ResultLog("Merged", l0, l1);
            Assert.Equal("Merged",l.Caption);
            Assert.Equal(3, l.Count);
            Assert.Equal(1, l.GoodCount);
            Assert.Equal(1, l.BadCount);
            Assert.Equal(1, l.SuspectCount);
            Assert.False(l.IsGood);
            Assert.False(l.IsSuspect);
            Assert.True((l.IsBad));
            Assert.False(l.IsNull);
        }

        [Fact]
        public void ClearTest()
        {
            ResultLog l0 = new ResultLog();
            l0.LogGood("Good result.");
            l0.LogBad("Error result.");
            l0.Clear();
            Assert.Empty(l0);
            Assert.Equal(0, l0.GoodCount);
            Assert.Equal(0, l0.BadCount);
            Assert.True(l0.IsNull);
        }

        [Fact]
        public void ToStringTest()
        {
            ResultLog l0 = new ResultLog();
            l0.LogGood("Good result.");
            l0.LogBad("Error result.");
            string s = l0.ToString();
            Assert.True(s.Contains("Good result") && s.Contains("Error result"));
        }

        [Fact]
        public void IListTests()
        {
            ResultLog l0 = new ResultLog();
            Result r0 = new Result(ResultType.Bad, "Error message");
            l0.Insert(0, r0);
            Assert.Contains<Result>(r0, l0);
            Assert.Equal(0, l0.IndexOf(r0));
            l0.Remove(r0);
            Assert.Empty(l0);
            l0.Add(r0);
            Assert.Single(l0);
            l0.RemoveAt(0);
            Assert.Empty(l0);
        }
    }
}
