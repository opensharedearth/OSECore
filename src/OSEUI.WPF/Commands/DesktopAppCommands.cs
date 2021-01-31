using System.Windows.Input;

namespace OSEUI.WPF.Commands
{
    public class DesktopAppCommands
    {
        public static readonly RoutedUICommand ExitApplication =
            new RoutedUICommand("Exit", "Exit", typeof(DesktopAppCommands));
        public static readonly RoutedUICommand OpenRecentDocument = new RoutedUICommand("Open Recent Document", "OpenRecent", typeof(DesktopAppCommands));
        public static readonly RoutedUICommand ShowResultLogFormCommand = new RoutedUICommand("Show Result Log", "ShowResultLog", typeof(DesktopAppCommands));
        public static readonly RoutedUICommand ShowAboutBoxCommand =
            new RoutedUICommand("Show About Box", "ShowAboutBox", typeof(DesktopAppCommands));
        public static readonly RoutedUICommand ShowSettingsCommand =
            new RoutedUICommand("Show Settings", "ShowSettings", typeof(DesktopAppCommands));
        public static readonly RoutedUICommand ZoomIn = new RoutedUICommand("Zoom In", "ZoomIn", typeof(DesktopAppCommands), new InputGestureCollection(new InputGesture[] { new MouseGesture(MouseAction.WheelClick, ModifierKeys.Control) }));
        public static readonly RoutedUICommand ZoomOut = new RoutedUICommand("Zoom Out", "ZoomOut", typeof(DesktopAppCommands), new InputGestureCollection(new InputGesture[] { new MouseGesture(MouseAction.WheelClick, ModifierKeys.Control) }));
        public static readonly RoutedUICommand ResetZoom = new RoutedUICommand("Reset Zoom", "ResetZoom", typeof(DesktopAppCommands));
        public static readonly RoutedUICommand SetZoom = new RoutedUICommand("Set Zoom", "SetZoom", typeof(DesktopAppCommands));
    }
}