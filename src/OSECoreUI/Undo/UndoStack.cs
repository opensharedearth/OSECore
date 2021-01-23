using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.Undo
{
    public class UndoStack : Stack<UndoObject>, IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposable)
        {
            if(disposable)
            {
                while (Count > 0)
                {
                    UndoObject uo = Pop();
                    uo.Dispose();
                }
            }
        }
        public new void Clear()
        {
            while(Count > 0)
            {
                UndoObject uo = Pop();
                uo.Dispose();
            }
        }
        public void PushOrCollapse(UndoObject uo)
        {
            if (Count > 0 && Peek().Collapse(uo))
            {
                ;
            }
            else
            {
                Push(uo);
            }

        }
    }
}
