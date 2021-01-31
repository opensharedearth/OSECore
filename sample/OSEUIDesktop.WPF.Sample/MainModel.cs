using OSECore.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSEUIDesktop.WPF.Sample
{
    public class MainModel
    {
        public static MainModel Instance { get; } = new MainModel();
        private ResultLog _log = new ResultLog();
        public ResultLog Log => _log;
        public SampleDocument Document { get; set; }
    }
}
