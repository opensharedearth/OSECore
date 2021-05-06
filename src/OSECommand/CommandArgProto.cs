using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class CommandArgProto : CommandArg
    {
        public const char NoMnemonic = '\0';
        public Usage Usage { get; } = null;
        public string Description { get; } = "";
        public bool IsRequired => (Options & CommandArgOptions.IsRequired) != 0;
        public bool HasMultiple => (Options & CommandArgOptions.HasMultiple) != 0;
        public bool HasArgument => (Options & CommandArgOptions.HasArgument) != 0;
        public override bool IsSwitch => (Options & CommandArgOptions.IsPositional) == 0;
        public override bool IsPositional => (Options & CommandArgOptions.IsPositional) != 0;
        public override bool IsMnemonic => base.IsMnemonic;
        private ArgValidator _validator = null;
        public CommandArgOptions Options { get; } = CommandArgOptions.None;
        public CommandArgProto(string name, char mnemonic = '\0', Usage usage = null, string value = null, ArgValidator validator = null, CommandArgOptions options = CommandArgOptions.None)
            : base(name, mnemonic, value)
        {
            Usage = usage ?? Usage.Null;
            _validator = validator ?? new ArgValidator();
            Options = options & ~CommandArgOptions.IsPositional;
        }
        public CommandArgProto(string name, int index, Usage usage = null, string value = null, ArgValidator validator = null, CommandArgOptions options = CommandArgOptions.None)
            : base(index, name)
        {
            Usage = usage ?? Usage.Null;
            _validator = validator ?? new ArgValidator();
            Options = options | CommandArgOptions.IsPositional;
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
