using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.Logging;

namespace OSECoreUI.Undo
{
    public class UndoSequence : UndoObject
    {
        bool _reverse = false;
        List<UndoObject> _script = null;
        UndoStack _stack = new UndoStack();
        public UndoSequence(string title, UndoStack stack, bool reverse = false)
            : base(title)
        {
            _reverse = reverse;
            _script = new List<UndoObject>(stack);
        }
        public override Result Undo()
        {
            if(_reverse)
            {
                foreach (UndoObject uo in _script.ToArray().Reverse())
                {
                    Result r = uo.Undo();
                    if (r.Type != ResultType.Good)
                    {
                        return r;
                    }
                }
            }
            else
            {
                foreach (UndoObject uo in _script.ToArray())
                {
                    Result r = uo.Undo();
                    if (r.Type != ResultType.Good)
                    {
                        return r;
                    }
                }
            }
            return new Result(ResultType.Good, "");
        }
        public override Result Redo()
        {
            if(_reverse)
            {
                foreach (UndoObject uo in _script.ToArray())
                {
                    Result r = uo.Redo();
                    if (r.Type != ResultType.Good)
                    {
                        return r;
                    }
                }
            }
            else
            {
                foreach (UndoObject uo in _script.ToArray().Reverse())
                {
                    Result r = uo.Redo();
                    if (r.Type != ResultType.Good)
                    {
                        return r;
                    }
                }
            }
            return new Result(ResultType.Good, "");
        }
    }
}
