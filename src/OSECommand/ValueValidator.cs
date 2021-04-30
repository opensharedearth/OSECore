using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.Text;

namespace OSECommand
{
    public class ValueValidator<T> : ArgValidator where T : struct
    {
        private T _defaultValue;
        public ValueValidator(T defaultValue = default(T))
        {
            _defaultValue = defaultValue;
        }
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result = base.Validate(arg);
            try
            {
                if (result.Succeeded)
                {
                    arg.ResolvedValue = arg.Value.GetValue<T>();
                }
            }
            catch(Exception ex)
            {
                result.Append(new CommandResult(false, $"Invalid value for {arg.Name}", ex));
            }
            return result;
        }
    }
}
