using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECommand
{
    public class UsageProto : UsageElement
    {
        public string Proto { get; set; } = "";
        public UsageProto(string proto, string description = "")
            : base(description)
        {
            if (proto == null) throw new ArgumentNullException("Proto cannot be null");
            Proto = proto;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Proto);
            if(HasDescription)
            {
                sb.Append('\t');
                sb.Append(Description);
            }
            return sb.ToString();
        }
        public override UsageType GetUsageType()
        {
            return UsageType.Proto;
        }
        public override string GetHeading()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Usage:");
            return sb.ToString();
        }
    }
}
