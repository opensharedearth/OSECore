using OSECore.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OSECore.Test.Types
{
    public class UserTypesTests
    {
        [Fact]
        public void IDictionaryTests()
        {
            string n0 = "user type";
            var t = new UserTypes();
            var ut0 = new UserType(n0, typeof(string), null);
            t.Add(n0, ut0);
            Assert.Single(t);
            Assert.Equal(ut0, t[n0]);
            Assert.True(t.ContainsKey(n0));
            Assert.True(t.Remove(n0));
            Assert.Empty(t);
        }

    }
}
