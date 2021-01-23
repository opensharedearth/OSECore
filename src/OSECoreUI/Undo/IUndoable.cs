using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Logging;

namespace OSECoreUI.Undo
{
    public interface IUndoable
    {
        IUndoRedo UndoRedo { get; set; }
    }
}
