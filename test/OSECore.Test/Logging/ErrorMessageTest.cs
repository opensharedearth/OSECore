using OSECore.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OSECore.Test.Logging
{
    public class ErrorMessageTest
    {
        [Fact]
        public void CtorTest()
        {
            string s0 = "error";
            var t = new ErrorMessage(s0);
            Assert.True(t.HasError);
            Assert.Equal(s0, t.Message);
            Assert.Equal(s0, t.GetErrorMessage());
            t.Clear();
            Assert.False(t.HasError);
            t.Message = s0;
            Assert.Equal(s0, t.Message);
        }
    }
}
