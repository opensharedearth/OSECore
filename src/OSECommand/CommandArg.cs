using System;
using System.Collections.Generic;
using System.Text;

namespace OSECommand
{
    public class CommandArg : IEquatable<CommandArg>
    {
        public static CommandArg Null = new CommandArg();
        public string Name { get; } = "";
        public char Mnemonic { get; } = '\0';
        public string Value { get; } = null;
        private object _resolvedValue = null;
        public object ResolvedValue { 
            get => _resolvedValue ?? Value;
            set => _resolvedValue = value; 
        }
        public virtual bool IsPositional => !IsSwitch && !IsNull;
        public virtual bool IsSwitch { get; } = false;
        public virtual bool IsMnemonic => Mnemonic != '\0';
        public bool IsNull => String.IsNullOrEmpty(Name);
        public int PositionIndex { get; } = -1;
        public CommandArg()
        {

        }
        public CommandArg(char c, string value = null)
        {
            Mnemonic = c;
            Value = value;
            IsSwitch = true;
        }
        public CommandArg(string name, string value = null)
        {
            Name = name;
            Value = value;
            IsSwitch = true;
        }
        public CommandArg(string name, char mnemonic, string value = null)
        {
            Name = name;
            Mnemonic = mnemonic;
            Value = value;
            IsSwitch = true;
        }
        public CommandArg(int index, string value)
        {
            Name = $"Arg{index}";
            Value = value;
            PositionIndex = index;
        }

        public bool Equals(CommandArg other)
        {
            if (other == null) return false;
            return Name == other.Name && Value == other.Value && IsSwitch == other.IsSwitch;
        }
        public override bool Equals(object obj)
        {
            if (obj is CommandArg arg) return Equals(arg);
            return base.Equals(obj);
        }
        public override string ToString()
        {
            if (IsMnemonic)
                return "-" + Mnemonic + (Value == null ? "" : " " + Value.ToString());
            else if (IsSwitch)
                return "--" + Name + (Value == null ? "" : "=" + Value.ToString());
            else
                return (Value == null ? "" : Value.ToString());
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ (Value?.GetHashCode() ?? 0) ^ IsSwitch.GetHashCode();
        }
        public bool MatchName(string name)
        {
            if(!String.IsNullOrEmpty(name) && IsSwitch)
            {
                return String.Compare(Name, name, true) == 0;
            }
            return false;
        }
    }
}
