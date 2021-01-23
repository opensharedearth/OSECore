using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OSEUIForms.WPF.Commands
{
    public class FormsCommands
    {
        public static RoutedUICommand ShowResultLogFormCommand = new RoutedUICommand("Show Result Log","ShowResultLog",typeof(FormsCommands));

        public static RoutedUICommand ShowAboutBoxCommand =
            new RoutedUICommand("Show About Box", "ShowAboutBox", typeof(FormsCommands));

        public static RoutedUICommand ShowSettingsCommand =
            new RoutedUICommand("Show Settings", "ShowSettings", typeof(FormsCommands));
    }
}
