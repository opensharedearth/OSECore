using OSECore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class ArgValidator
    {
        public virtual CommandResult Validate(CommandArg arg)
        {
            if (String.IsNullOrEmpty(arg.Name) && arg.Mnemonic == '\0')
                return new CommandResult(false, $"Invalid argument in command line");
            else
                return new CommandResult();
    }
}
}
