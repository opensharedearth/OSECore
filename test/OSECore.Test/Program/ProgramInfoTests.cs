using OSECore.Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OSECore.Test.Program
{
    public class ProgramInfoTests
    {
        [Fact]
        public void GetProgramNameTest()
        {
            string s0 = ProgramInfo.GetProgramName();
            Assert.NotNull(s0);
        }
        [Fact]
        public void GetProgramVersionTest()
        {
            string s0 = ProgramInfo.GetProgramVersion();
            Assert.NotNull(s0);
        }
        [Fact]
        public void GetProgramDescriptionTest()
        {
            string s0 = ProgramInfo.GetProgramDescription();
            Assert.NotNull(s0);
        }

    }
}
