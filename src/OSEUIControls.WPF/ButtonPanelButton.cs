using System.Windows;
using System.Windows.Input;

namespace OSEUIControls.WPF
{
    public class ButtonPanelButton
    {

        public string Label { get; set; } = "";

        public string Tag { get; set; } = "";

        public bool IsEnabled { get; set; } = true;

        public bool IsDefault { get; set; } = false;
        public bool IsCancel { get; set; } = false;

        public ICommand Command { get; set; } = null;

        public object CommandParameter { get; set; } = null;
    }
}