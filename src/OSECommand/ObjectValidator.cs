using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.Text;

namespace OSECommand
{
    public class ObjectValidator<T> : ArgValidator where T : class
    {
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result = base.Validate(arg);
            try
            {
                arg.ResolvedValue = arg.Value.GetObject<T>();
            }
            catch(Exception ex)
            {
                result.Append(new CommandResult(false, $"Unable to construct {typeof(T).Name}", ex));
            }
            return result;
        }
    }
}
