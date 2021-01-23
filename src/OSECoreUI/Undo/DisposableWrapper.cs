using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.Undo
{
    public class DisposableWrapper : IDisposable
    {
        IDisposable _disposable = null;
        public DisposableWrapper(IDisposable disposable)
        {
            _disposable = disposable;
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
                if (_disposable != null)
                {
                    _disposable.Dispose();
                }
            }
        }
    }
}
