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
            Assert.Equal(l.Count, 0);
            Assert.Equal(l.GoodCount, 0);
            Assert.Equal(l.BadCount, 0);
            Assert.Equal(l.SuspectCount, 0);
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
            Assert.Equal(l.Caption, "Merged");
            Assert.Equal(l.Count, 3);
            Assert.Equal(l.GoodCount, 1);
            Assert.Equal(l.BadCount, 1);
            Assert.Equal(l.SuspectCount, 1);
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
            Assert.Equal(l0.Count, 0);
            Assert.Equal(l0.GoodCount, 0);
            Assert.Equal(l0.BadCount, 0);
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

    }
}
