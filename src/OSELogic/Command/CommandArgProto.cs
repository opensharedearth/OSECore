using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class CommandArgProto : CommandArg
    {
        public Usage Usage { get; } = Usage.Null;
        public string Description { get; } = "";
        public bool IsRequired => (Options & CommandArgOptions.IsRequired) != 0;
        public bool HasMultiple => (Options & CommandArgOptions.HasMultiple) != 0;
        public bool HasArgument => (Options & CommandArgOptions.HasArgument) != 0;
        public override bool IsSwitch => (Options & CommandArgOptions.IsPositional) == 0;
        public override bool IsPositional => (Options & CommandArgOptions.IsPositional) != 0;
        private ArgValidator _validator = null;
        public CommandArgOptions Options { get; } = CommandArgOptions.None;
        public CommandArgProto(string name, Usage usage, string value = null, ArgValidator validator = null, CommandArgOptions options = CommandArgOptions.None)
            : base(name, value)
        {
            Usage ??= usage;
            _validator = validator ?? new ArgValidator();
            Options = options;
        }
        public CommandResult Validate(string field)
        {
            return _validator.Validate(new CommandArg(Name, field));
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
