using System.Windows.Input;

namespace OSEUI.WPF.Commands
{
    public class DesktopAppCommands
    {
        public static RoutedUICommand ExitApplication =
            new RoutedUICommand("Exit", "Exit", typeof(DesktopAppCommands));
        public static RoutedUICommand OpenRecentDocument = new RoutedUICommand("Open Recent Document", "OpenRecent", typeof(DesktopAppCommands));
        public static RoutedUICommand ShowResultLogFormCommand = new RoutedUICommand("Show Result Log", "ShowResultLog", typeof(DesktopAppCommands));

        public static RoutedUICommand ShowAboutBoxCommand =
            new RoutedUICommand("Show About Box", "ShowAboutBox", typeof(DesktopAppCommands));

        public static RoutedUICommand ShowSettingsCommand =
            new RoutedUICommand("Show Settings", "ShowSettings", typeof(DesktopAppCommands));

    }
}