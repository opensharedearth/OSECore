using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OSECommand
{
    public class NameValidator : ArgValidator
    {
        int _maxLength;
        string _namePattern;
        public NameValidator(int maxLength = 511, string namePattern = "^[a-zA-Z_][0-9a-zA-Z_]*$")
        {
            _maxLength = maxLength;
            _namePattern = namePattern;
        }
        public override CommandResult Validate(CommandArg arg)
        {
            CommandResult result =  base.Validate(arg);
            if(result.Succeeded)
            {
                if(arg.Value.Length > _maxLength || !Regex.IsMatch(arg.Value, _namePattern))
                {
                    result.Append(new CommandResult(false, $"{arg.Name} name is invalid"));
                }
            }
            return result;
        }
    }
}
