using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class HostValidator : ArgValidator
    {
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result =  base.Validate(arg);
            if(result.Succeeded)
            {
                if(Uri.TryCreate("tcp://" + arg.Value, UriKind.Absolute, out Uri uri))
                {
                    if(uri.GetComponents(UriComponents.HostAndPort, UriFormat.Unescaped) == arg.Value)
                    {
                        return result;
                    }
                }
            }
            return new CommandResult(false, $"'{arg.Value}' is not a valid URL");
        }
    }
}
