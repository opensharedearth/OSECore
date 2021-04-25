using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSEExcelAdaptor
{
    public class WorksheetReference
    {
        public WorksheetReference()
        {

        }
        public WorksheetReference(int index)
        {
            Index = index;
        }
        public WorksheetReference(string name)
        {
            if(int.TryParse(name, out int index))
            {
                Index = index;
            }
            else
            {
                Name = name;
            }
        }
        public int Index = -1;
        public string Name = null;
        public object Reference => Name == null ? (object)Index : Name;
        public string Description => Name == null ? Index.ToString() : Name;
        public bool HasName => Name != null;
    }
}
