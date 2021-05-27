using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSETesting
{
    public enum OSCompatibility
    {
        None = 0,
        Windows = 1,
        Linux = 2,
        OSX = 4,
        FreeBSD = 8,
        AllUnix = 14,
        Any = 15
    }
}
