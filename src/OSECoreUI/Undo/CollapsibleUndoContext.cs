using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSECoreUI.Undo
{
    public class CollapsibleUndoContext : UndoContext
    {
        ContextReference _contextReference = null;
        public CollapsibleUndoContext(ContextReference contextReference, params object[] objects)
            : base(objects)
        {
            _contextReference = contextReference;
        }
        public ContextReference ContextReference
        {
            get
            {
                return _contextReference;
            }
        }
    }
}
