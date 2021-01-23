using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.App
{
    public interface IUIStatus
    {
        string Status { get; set; }
        void StartProgress(int nsteps, string status = "");
        void NextStep(int steps = 1);
        void StopProgress(string status = "Ready");
    }
}
