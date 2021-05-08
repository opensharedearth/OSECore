using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class UsageCommand : UsageElement
    {
        public string Name { get; private set; }
        public UsageCommand(string name, string description)
            : base(description)
        {
            if (name == null) throw new ArgumentNullException("Command name cannot be null");
            Name = name;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append(GetPadding(sb.Length + 1));
            sb.Append(Description);
            return sb.ToString();
        }
        public override UsageType GetUsageType()
        {
            return UsageType.Command;
        }
        public override string GetHeading()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Commands:");
            return sb.ToString();
        }
    }
}
