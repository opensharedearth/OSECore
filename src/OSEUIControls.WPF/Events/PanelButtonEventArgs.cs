using System;
using System.Windows;

namespace OSEUIControls.WPF.Events
{
    public class PanelButtonEventArgs : RoutedEventArgs
    {
        public string ButtonTag { get; }
        public PanelButtonEventArgs(string buttonTag)
        : base(ButtonPanel.ButtonPressedEvent)
        {
            ButtonTag = buttonTag;
        }
    }
}