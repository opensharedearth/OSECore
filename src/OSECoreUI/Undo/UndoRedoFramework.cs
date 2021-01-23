using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.Logging;

namespace OSECoreUI.Undo
{
    public class UndoRedoFramework : IDisposable, IUndoRedo
    {
        int _disableCount = 0;
        public void Disable()
        {
            _disableCount++;
        }
        public void Enable()
        {
            if (_disableCount > 0)
                _disableCount--;
        }
        public bool IsEnabled
        {
            get
            {
                return _disableCount == 0;
            }
        }
        UndoStack _undoStack = new UndoStack();
        UndoStack _redoStack = new UndoStack();
        Stack<UndoStack> _undoStacks = new Stack<UndoStack>();
        public Result Undo()
        {
            if(_undoStack.Count > 0)
            {
                UndoObject uo = _undoStack.Pop();
                Disable();
                Result r = uo.Undo();
                Enable();
                if(r.Type == ResultType.Good && IsEnabled)
                {
                    _redoStack.Push(uo);
                }
                return r;
            }
            else
            {
                return new Result(ResultType.Bad, "Nothing to undo.");
            }
        }
        public Result Redo()
        {
            if (_redoStack.Count > 0)
            {
                UndoObject uo = _redoStack.Pop();
                Disable();
                Result r = uo.Redo();
                Enable();
                if (r.Type == ResultType.Good && IsEnabled)
                {
                    _undoStack.Push(uo);
                }
                return r;
            }
            else
            {
                return new Result(ResultType.Bad, "Nothing to redo.");
            }
        }
        public void PushUndo(UndoObject uo)
        {
            if(IsEnabled)
            {
                _undoStack.PushOrCollapse(uo);
                _redoStack.Clear();
            }
        }

        public void StartSequence()
        {
            if(IsEnabled)
            {
                _undoStacks.Push(_undoStack);
                _undoStack = new UndoStack();
            }
        }
        public UndoSequence EndSequence(string title, bool reverse = false)
        {
            if(_undoStacks.Count > 0 && IsEnabled)
            {
                UndoSequence uso = new UndoSequence(title, _undoStack, reverse);
                _undoStack = _undoStacks.Pop();
                _undoStack.Push(uso);
                return uso;
            }
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposable)
        {
            if(disposable)
            {
                _redoStack.Dispose();
                _undoStack.Dispose();
                while (_undoStacks.Count > 0)
                {
                    UndoStack stack = _undoStacks.Pop();
                    stack.Dispose();
                }
            }
        }

        public string UndoTitle
        {
            get
            {
                if(_undoStack.Count > 0)
                {
                    return "Undo " +_undoStack.Peek().Title;
                }
                return "Undo";
            }
        }
        public string RedoTitle
        {
            get
            {
                if (_redoStack.Count > 0)
                {
                    return "Redo " + _redoStack.Peek().Title;
                }
                return "Redo";
            }
        }
        public bool HasUndo
        {
            get
            {
                return _undoStack.Count > 0;
            }
        }
        public bool HasRedo
        {
            get
            {
                return _redoStack.Count > 0;
            }
        }
        public void Clear()
        {
            _redoStack.Clear();
            _undoStack.Clear();
            while(_undoStacks.Count > 0)
            {
                _undoStacks.Pop().Dispose();
            }
        }
    }
}
