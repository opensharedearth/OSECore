using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OSEUIForms.WPF.App
{
    public class SettingsPageSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SettingsPageDefinition spd)
            {
                return Application.Current.FindResource(spd.ResourceName) as DataTemplate;
            }

            return null;
        }
    }
}
