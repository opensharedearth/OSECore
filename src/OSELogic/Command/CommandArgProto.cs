using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class CommandArgProto : CommandArg
    {
        public string Description { get; } = "";
        public bool IsRequired => (Options & CommandArgOptions.IsRequired) != 0;
        public bool HasMultiple => (Options & CommandArgOptions.HasMultiple) != 0;
        private ArgValidator _validator = new ArgValidator();
        private CommandArgOptions Options { get; } = CommandArgOptions.None;
        public CommandArgProto(string name, string description, string value = null, ArgValidator validator = null, CommandArgOptions options = CommandArgOptions.None)
            : base(name, value)
        {
            Description = description;
            Value = value;
            if (validator != null) _validator = validator;
        }
        public CommandResult Validate(CommandArg arg)
        {
            return _validator.Validate(arg);
        }
        public CommandArg Resolve(string field)
        {
            return Resolve(new CommandArg(Name, field));
        }
        public CommandArg Resolve(CommandArg arg)
        {
            var result = Validate(arg);
            if (!result.Succeeded) throw new ArgumentException(result.ToString());
            return arg;
        }
    }
}
