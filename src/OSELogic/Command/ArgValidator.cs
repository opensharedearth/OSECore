﻿using OSECore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class ArgValidator
    {
        public virtual CommandResult Validate(CommandArg arg)
        {
            if (!String.IsNullOrEmpty(arg.Name))
            {
                return new CommandResult();
            }
            else
                return new CommandResult(false, $"{arg.Name} cannot be empty.");
        }
    }
}
