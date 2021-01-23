using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.App
{
    public interface IDirty
    {
        void Dirty();
        void Undirty();
        bool IsDirty { get; }
    }
}
