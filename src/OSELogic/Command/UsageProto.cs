﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSELogic.Command
{
    public class UsageProto : UsageElement
    {
        public string Proto { get; set; } = "";
        public UsageProto(string proto)
        {
            Proto = proto;
        }
    }
}
