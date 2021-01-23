using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using OSECore.Text;
using OSECoreUI.Annotations;
using OSECoreUI.Annotations.Forms;
using OSEUIControls.WPF.Events;

namespace OSEUIControls.WPF
{
    public class ButtonPanel : Control, INotifyPropertyChanged
    {
        public string OKLabel { get; set; } = "OK";
        public const string OKTag = "OK";
        public string CancelLabel { get; set; } = "Cancel";
        public const string CancelTag = "Cancel";
        public ICommand OKCommand { get; set; } = null;
        public object OKCommandParameter { get; set; } = null;
        public ICommand CancelCommand { get; set; } = null;
        public object CancelCommandParameter { get; set; } = null;
        
        static ButtonPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonPanel), new FrameworkPropertyMetadata(typeof(ButtonPanel)));
        }

        public ButtonPanel()
        {
            CommandBindings.Add(new CommandBinding(EnableButtonCommand, EnableButtonHandler, CanEnableButtonHandler));
            CommandBindings.Add(new CommandBinding(DisableButtonCommand, DisableButtonHandler, CanDisableButtonHandler));
        }

        private void CanEnableButtonHandler(object sender, CanExecuteRoutedEventArgs args)
        {
            if (args.Parameter is string tag)
            {
                args.CanExecute = GetButton(tag) != null;
            }
        }

        private void EnableButtonHandler(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Parameter is string tag)
            {
                ChangeButtonEnable(tag, true);
                args.Handled = true;
            }
        }

        private void CanDisableButtonHandler(object sender, CanExecuteRoutedEventArgs args)
        {
            if (args.Parameter is string tag)
            {
                args.CanExecute = GetButton(tag) != null;
            }
        }

        private void DisableButtonHandler(object sender, ExecutedRoutedEventArgs args)
        {
            if (args.Parameter is string tag)
            {
                ChangeButtonEnable(tag, false);
                args.Handled = true;
            }
        }

        public void ChangeButtonEnable(string tag, bool enable)
        {
            Button b = GetButton(tag);
            if (b != null) b.IsEnabled = enable;

        }

        public override void OnApplyTemplate()
        {
            SetupPanel();
            base.OnApplyTemplate();
        }

        public static readonly RoutedUICommand EnableButtonCommand = new RoutedUICommand("Enable Button", "EnableButton", typeof(ButtonPanel));
        public static readonly RoutedUICommand DisableButtonCommand = new RoutedUICommand("Disable Button", "DisableButton", typeof(ButtonPanel));
        public delegate void PanelButtonEventHandler(object sender, PanelButtonEventArgs args);
        public static readonly RoutedEvent ButtonPressedEvent = EventManager.RegisterRoutedEvent("Button Pressed",
            RoutingStrategy.Bubble, typeof(PanelButtonEventHandler), typeof(ButtonPanel));
        public static readonly  DependencyProperty IsDefaultButtonEnabledProperty = DependencyProperty.Register("IsDefaultButtonEnabled",typeof(bool), typeof(ButtonPanel), 
            new PropertyMetadata(true, new PropertyChangedCallback(OnDefaultButtonEnabledChanged)));

        private static void OnDefaultButtonEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ButtonPanel bp && dependencyPropertyChangedEventArgs.NewValue is bool enabled && bp.DefaultButton != null)
            {
                bp.DefaultButton.IsEnabled = enabled;
            }
        }

        public static readonly DependencyProperty IsCancelButtonEnabledProperty = DependencyProperty.Register("IsCancelButtonEnabled", typeof(bool), typeof(ButtonPanel), 
            new PropertyMetadata(true, new PropertyChangedCallback(OnCancelButtonEnabledChanged)));

        private static void OnCancelButtonEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyObject is ButtonPanel bp && dependencyPropertyChangedEventArgs.NewValue is bool enabled && bp.DefaultButton != null)
            {
                bp.DefaultButton.IsEnabled = enabled;
            }
        }

        public event PanelButtonEventHandler ButtonPressed
        {
            add => AddHandler(ButtonPressedEvent, value);
            remove => AddHandler(ButtonPressedEvent, value);
        }

        public bool HasOk { get; set; } = true;

        public bool HasCancel { get; set; } = true;
        public Style ButtonStyle { get; set; }

        public bool IsDefaultButtonEnabled
        {
            get => (bool) GetValue(IsDefaultButtonEnabledProperty);
            set => SetValue(IsDefaultButtonEnabledProperty, value);
        }

        public bool IsCancelButtonEnabled
        {
            get => (bool) GetValue(IsCancelButtonEnabledProperty);
            set => SetValue(IsCancelButtonEnabledProperty, value);
        }


        public ButtonOrder ButtonOrder { get; set; } = ButtonOrder.OKCancel;


        public Button GetButton(string tag)
        {
            StackPanel sp = Template.FindName("PART_Panel", this) as StackPanel;
            foreach (Button b in sp.Children.OfType<Button>())
            {
                if (b.Tag is string s)
                {
                    if (s == tag) return b;
                }
            }

            return null;
        }

        private Button _defaultButton = null;
        private Button _cancelButton = null;
        public List<ButtonPanelButton> Buttons { get; set; } = new List<ButtonPanelButton>();

        public Button DefaultButton => _defaultButton;

        public Button CancelButton => _cancelButton;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        private void SetupPanel()
        {
            if (Template.FindName("PART_Panel", this) is StackPanel sp)
            {
                if (ButtonOrder == ButtonOrder.OKCancel && HasOk) AddButton(sp, OKLabel, OKTag, IsDefaultButtonEnabled, true, false, OKCommand, OKCommandParameter);
                if (ButtonOrder == ButtonOrder.CancelOK && HasCancel) AddButton(sp, CancelLabel, CancelTag, IsCancelButtonEnabled, false, true, CancelCommand, CancelCommandParameter);
                foreach (ButtonPanelButton bpb in Buttons)
                {
                    AddButton(sp, bpb.Label, bpb.Tag, bpb.IsEnabled, bpb.IsDefault, bpb.IsCancel, bpb.Command, bpb.CommandParameter);
                }

                if (ButtonOrder == ButtonOrder.CancelOK && HasOk) AddButton(sp, OKLabel, OKTag, IsDefaultButtonEnabled, true, false, OKCommand, OKCommandParameter);
                if (ButtonOrder == ButtonOrder.OKCancel && HasCancel) AddButton(sp, CancelLabel, CancelTag, IsCancelButtonEnabled, false, true, CancelCommand, CancelCommandParameter);
            }
            else
            {
                throw new ApplicationException("No 'PART_Panel' defined for button panel.");
            }
        }

        private Button AddButton(StackPanel sp, string label, string tag, bool isEnabled = true, bool isDefault = false, bool isCancel = false, ICommand command = null, object commandParameter = null)
        {
            Button b = new Button()
            {
                Content = label,
                Tag = String.IsNullOrEmpty(tag) ? label : tag,
                IsEnabled = isEnabled,
                Command = command,
                CommandParameter = commandParameter
            };
            if (isDefault)
            {
                b.IsDefault = true;
                if(_defaultButton != null)throw new ApplicationException("More than one default button defined for button panel.");
                _defaultButton = b;
            }

            if (isCancel)
            {
                b.IsCancel = true;
                if(_cancelButton != null)throw new ApplicationException("More than one cancel button defined for button panel.");
                _cancelButton = b;
            }
            b.Click += Button_Click;
            b.Style = GetButtonStyle();
            sp.Children.Add(b);
            return b;
        }

        private Style GetButtonStyle()
        {
            if(ButtonStyle != null)
            {
                return ButtonStyle;
            }
            else
            {
                ComponentResourceKey key = new ComponentResourceKey(typeof(ButtonPanel), "ButtonStyle");
                return FindResource(key) as Style;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button b)
            {
                RaiseEvent(new PanelButtonEventArgs(b.Tag as string));
            }
        }
    }
}
