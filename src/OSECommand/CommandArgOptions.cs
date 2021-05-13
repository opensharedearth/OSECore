using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    [Flags]
    public enum CommandArgOptions
    {
        /// <summary>   No options specified. </summary>
        None = 0,

        /// <summary>   Argument is positional.  A positional argument is indicated
        ///             by its position on the deck statement line.  If a argument is not positional it must be a switch
        ///             </summary>
        IsPositional = 1,

        /// <summary>   Argument is required.  Required fields must be present in a command line.
        ///              </summary>
        IsRequired = 2,

        /// <summary>   Multiple arguments of this name are permitted. </summary>
        HasMultiple = 4,
        HasArgument = 8,
        IsCommand = 16
    }
}
