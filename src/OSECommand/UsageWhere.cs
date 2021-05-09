using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class UsageWhere : UsageElement
    {
        public UsageWhere(string name, string description)
            : base(description)
        {
            Name = name;
        }
        public string Name { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Name);
            sb.Append('\t');
            sb.Append(Description);
            return sb.ToString();
        }
        public override UsageType GetUsageType()
        {
            return UsageType.Where;
        }
        public override string GetHeading()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Where:");
            return sb.ToString();
        }
    }
}
