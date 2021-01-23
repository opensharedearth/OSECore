using System;
using System.Collections.Generic;
using System.Text;
using OSECore.Logging;

namespace OSECoreUI.Undo
{
    public interface IUndoRedo
    {
        Result Undo();
        Result Redo();
        void PushUndo(UndoObject uo);
        void StartSequence();
        UndoSequence EndSequence(string title, bool reverse = false);
        string UndoTitle { get; }
        string RedoTitle { get; }
        bool HasUndo { get; }
        bool HasRedo { get; }
        void Clear();
    }
}
