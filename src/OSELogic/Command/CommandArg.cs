using System;
using System.Collections.Generic;
using System.Text;

namespace OSELogic.Command
{
    public class CommandArg
    {
        public static CommandArg Null = new CommandArg();
        public string Name { get; } = "";
        public string Value { get; set; } = null;
        public bool IsSwitch { get; } = false;
        public bool IsNull => String.IsNullOrEmpty(Name);
        public CommandArg()
        {

        }
        public CommandArg(string name, string value)
        {
            Name = name;
            Value = value;
        }
        public CommandArg(string name)
        {
            Name = name;
            IsSwitch = true;
        }
    }
}
