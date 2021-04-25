using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.IO;
using OSELogic.Config;

namespace OSELogic.Command
{
    public class PathValidator : ArgValidator
    {
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result = base.Validate(arg);
            string path = arg.Value;
            if (!Path.IsPathFullyQualified(path))
            {
                try
                {
                    path = Path.Combine(GeneralParameters.Instance.WorkingFolder, path);
                }
                catch (Exception ex)
                {
                    result.Append(new CommandResult(false, "Invalid path:", ex));
                }
            }
            if (result.Succeeded)
            {
                if(!FileSupport.IsFileReadable(path))
                {
                    return new CommandResult(false, $"File '{path}' is not readable.");
                }
                arg.ResolvedValue = path;
            }
            return result;
        }
    }
}
