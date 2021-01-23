using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OSECore.Logging;

namespace OSECoreUI.Undo
{
    public class UndoObject : IDisposable
    {
        string _title = "";
        UndoContext _undoContext = null;
        UndoContext _redoContext = null;
        Func<UndoContext, UndoContext> _undo = null;
        Action<UndoContext> _redo = null;
        public UndoObject(string title = "", UndoContext context = null, Func<UndoContext, UndoContext> undo = null, Action<UndoContext> redo = null)
        {
            _title = title;
            _undoContext = context;
            _undo = undo;
            _redo = redo;
        }
        public string Title
        {
            get
            {
                return _title;
            }
        }
        public virtual Result Undo()
        {
            try
            {
                _redoContext = _undo(_undoContext);
            }
            catch(Exception ex)
            {
                return new Result(ResultType.Bad, "Unable to undo " + _title + ":" + ex.Message);
            }
            return new Result(ResultType.Good, "");
        }
        public virtual Result Redo()
        {
            try
            {
                _redo(_redoContext);
                return new Result(ResultType.Good, "");
            }
            catch(Exception ex)
            {
                return new Result(ResultType.Bad, "Unable to redo " + _title + ":" + ex.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if (_redoContext != null)
                {
                    _redoContext.Dispose();
                }
                if (_undoContext != null)
                {
                    _undoContext.Dispose();
                }
            }
        }

        public bool IsCollapsible
        {
            get
            {
                return _undoContext is CollapsibleUndoContext;
            }
        }
        public virtual bool CanCollapse(UndoObject uo)
        {
            return IsCollapsible && uo != null && uo.IsCollapsible && (_undoContext as CollapsibleUndoContext).ContextReference == (uo._undoContext as CollapsibleUndoContext).ContextReference;
        }
        public virtual bool Collapse(UndoObject uo)
        {
            return CanCollapse(uo);
        }
    }
}
