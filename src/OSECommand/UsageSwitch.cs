using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class UsageSwitch : UsageElement
    {
        public string Name { get; private set; }
        public char Mnemonic { get; private set; }
        public bool HasName => !String.IsNullOrEmpty(Name);
        public bool HasMnemonic => Mnemonic != '\0';
        public UsageSwitch(string name, char mnemonic, string description)
            : base(description)
        {
            Name = name;
            Mnemonic = mnemonic;
        }
        public UsageSwitch(string name, string description)
        : base(description)
        {
            Name = name;
            Mnemonic = '\0';
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (HasName)
                sb.Append($"--{Name}");
            if(HasMnemonic)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append($"-{Mnemonic}");
            }
            sb.Append(GetPadding(sb.Length + 1));
            sb.Append(Description);
            return sb.ToString();
        }
        public override UsageType GetUsageType()
        {
            return UsageType.Switch;
        }
        public override string GetHeading()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Options:");
            return sb.ToString();
        }
    }
}
