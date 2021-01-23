using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace OSEUI.WPF.Resources
{
    public static class OSEUIResources
    {
        static Uri _resourceDictionaryUri = new Uri(@"/OSEUI.WPF;component/Resources/ResourceDictionary.xaml",UriKind.RelativeOrAbsolute);
        static ResourceDictionary _resources = null;
        public static ResourceDictionary Resources
        {
            get
            {
                if(_resources == null)
                {
                    _resources = LoadResources();
                }
                return _resources;
            }
        }
        static ResourceDictionary LoadResources()
        {
            ResourceDictionary rd = new ResourceDictionary();
            rd.Source = _resourceDictionaryUri;
            return rd;
        }
    }
}
