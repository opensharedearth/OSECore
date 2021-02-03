using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSEUIDesktop.WPF.Sample
{
    public static class SampleCommands
    {
        public static readonly RoutedUICommand AddEntry = new RoutedUICommand("Add Journal Entry", "AddEntry", typeof(SampleCommands));
        public static readonly RoutedUICommand EditEntry = new RoutedUICommand("Edit Journal Entry", "EditEntry", typeof(SampleCommands));
        public static readonly RoutedUICommand DeleteEntry = new RoutedUICommand("Remove Journal Entry", "DeleteEntry", typeof(SampleCommands));
        public static readonly RoutedUICommand AddImage = new RoutedUICommand("Add Image", "AddImage", typeof(SampleCommands));
        public static readonly RoutedUICommand DeleteImage = new RoutedUICommand("Remove Image", "DeleteImage", typeof(SampleCommands));
    }
}
