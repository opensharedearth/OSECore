using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Logging;

namespace OSECoreUI.App
{
    public class BaseAppContext
    {
        public BaseAppContext(ResultLog log)
        {
            Log = log;
        }

        public ResultLog Log { get; }
    }
}
