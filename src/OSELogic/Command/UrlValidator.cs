using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class UrlValidator : ArgValidator
    {
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result =  base.Validate(arg);
            if(result.Succeeded)
            {
                if(Uri.TryCreate(arg.Value.ToString(), UriKind.Absolute, out Uri uri))
                {
                    arg.ResolvedValue = uri;
                    return result;
                }
            }
            return new CommandResult(false, $"'{arg.Value}' is not a valid URL");
        }
    }
}
