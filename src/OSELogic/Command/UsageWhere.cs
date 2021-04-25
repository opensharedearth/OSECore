using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class UsageWhere : UsageElement
    {
        public UsageWhere(string name, string description)
        {
            Name = name;
            Description = description;
        }
        string Name { get; set; }
        string Description { get; set; }
    }
}
