using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathTool
{
    [Flags]
    public enum FolderStatus
    {
        Valid = 0,
        NullPath = 1,
        InvalidPath = 2,
        NotFullyQualified = 4,
        Nonextant = 8,
        Unreadable = 16,
        Duplicate = 32
    }
}
