using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.Undo
{
    public class UndoContext : IDisposable
    {
        public UndoContext(params object[] objects)
        {
            _objects = objects;
        }
        object[] _objects;
        public object this[int index]
        {
            get
            {
                if(index >= 0 && index < _objects.Length)
                {
                    return _objects[index];
                }
                else
                {
                    return null;
                }
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
                foreach (DisposableWrapper wrapper in _objects.OfType<DisposableWrapper>())
                {
                    wrapper.Dispose();
                }
            }
        }
    }
}
