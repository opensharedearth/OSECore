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
    public class FolderValidator : ArgValidator
    {
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result = base.Validate(arg);
            if(result.Succeeded)
            {
                string path = arg.Value;
                if (!Path.IsPathFullyQualified(path))
                {
                    try
                    {
                        path = Path.Combine(GeneralParameters.Instance.WorkingFolder, path);
                    }
                    catch(Exception ex)
                    {
                        result.Append(new CommandResult(false, "Invalid path:", ex));
                    }
                }
                if (result.Succeeded)
                {
                    if (!FileSupport.IsFolderReadable(path))
                    {
                        return new CommandResult(false, $"Folder '{path}' does not exist or is not readable");
                    }
                    arg.Value = path;
                }
            }
            return result;
        }
    }
}
