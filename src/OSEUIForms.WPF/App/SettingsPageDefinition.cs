using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSEUIForms.WPF.App
{
    public struct SettingsPageDefinition
    {
        public SettingsPageDefinition(string title, string resourceName, object context)
        {
            Title = title;
            ResourceName = resourceName;
            Context = context;
        }
        public string Title { get; set; }
        public string ResourceName { get; set; }
        public object Context { get; set; }
        public override string ToString()
        {
            return Title;
        }
    }
}
