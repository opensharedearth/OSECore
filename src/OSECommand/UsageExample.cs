using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class UsageExample
        : UsageElement
    {
        public string Example { get; private set; }
        public UsageExample(string example, string description)
            : base(description)
        {
            if (example == null) throw new ArgumentNullException("Example cannot be null");
            Example = example;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Example);
            if (HasDescription)
            {
                sb.Append(GetPadding(sb.Length + 1));
                sb.Append(Description);
            }
            return sb.ToString();
        }
        public override UsageType GetUsageType()
        {
            return UsageType.Example;
        }
        public override string GetHeading()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Examples:");
            return sb.ToString();
        }
    }
}
