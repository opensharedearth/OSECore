using OSECoreUI.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSECore.Logging;
using OSECoreUI.App;
using OSEUI.WPF.Commands;

namespace OSEUIControls.WPF
{
    public class StatusBar : Control
    {
        public static DependencyProperty ResultLogProperty = DependencyProperty.Register("ResultLog", typeof(ResultLog), typeof(StatusBar),
            new FrameworkPropertyMetadata(ResultLogChanged));
        public static DependencyProperty UIStatusProperty = DependencyProperty.Register("UIStatus", typeof(UIStatus), typeof(StatusBar),
            new FrameworkPropertyMetadata(UIStatusChanged));
        public static DependencyProperty UIZoomProperty = DependencyProperty.Register("UIZoom", typeof(UIZoom), typeof(StatusBar),
            new FrameworkPropertyMetadata(UIZoomChanged));
        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ZoomSlider.ValueChanged += ZoomSlider_ValueChanged;
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(DesktopAppCommands.SetZoom.CanExecute(e.NewValue, ZoomSlider))
            {
                DesktopAppCommands.SetZoom.Execute(e.NewValue, ZoomSlider);
            }
        }

        private static void ResultLogChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatusBar sb && e.NewValue is ResultLog log)
            {
                var view = sb.ResultLogView;
                view?.Dispose();
                sb.ResultLogView = new ResultLogView(log);
            }
        }
        private static void UIStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is StatusBar sb)
            {
                if (e.OldValue is UIStatus uis0)
                {
                    uis0.PropertyChanged -= sb.Uis_PropertyChanged;
                }
                if (e.NewValue is UIStatus uis1)
                {
                    uis1.PropertyChanged += sb.Uis_PropertyChanged;
                }
            }
        }

        private void Uis_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Status":
                    StatusText.Text = UIStatus.Status;
                    break;
                case "TotalSteps":
                    StatusProgress.Maximum = UIStatus.TotalSteps;
                    break;
                case "CurrentStep":
                    StatusProgress.Value = UIStatus.CurrentStep;
                    break;
                default:
                    break;
            }
        }

        static private void UIZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatusBar sb)
            {
                if (e.OldValue is UIZoom uiz0)
                {
                    uiz0.PropertyChanged -= sb.Uiz_PropertyChanged;
                }
                if (e.NewValue is UIStatus uiz1)
                {
                    uiz1.PropertyChanged += sb.Uiz_PropertyChanged;
                }
            }
        }

        private void Uiz_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MinZoomLevel":
                    ZoomSlider.Minimum = UIZoom.MinZoomLevel;
                    break;
                case "MaxZoomLevel":
                    ZoomSlider.Maximum = UIZoom.MaxZoomLevel;
                    break;
                case "ZoomLevel":
                    ZoomSlider.Value = UIZoom.ZoomLevel;
                    break;
                default:
                    break;
            }
        }

        public ResultLog ResultLog
        {
            get => (ResultLog)GetValue(ResultLogProperty);
            set => SetValue(ResultLogProperty, value);
        }
        public UIStatus UIStatus
        {
            get => (UIStatus)GetValue(UIStatusProperty);
            set => SetValue(UIStatusProperty, value);
        }
        public UIZoom UIZoom
        {
            get => (UIZoom)GetValue(UIZoomProperty);
            set => SetValue(UIZoomProperty, value);
        }


        public ResultLogView ResultLogView { get; set; }
        public bool HasZoom { get; set; } = false;
        public bool HasProgress { get; set; } = false;
        public double MinZoomLevel { get; set; } = 1.0;
        public double MaxZoomLevel { get; set; } = 5.0;
        public TextBlock StatusText => Template.FindName("StatusText", this) as TextBlock;
        public ProgressBar StatusProgress => Template.FindName("StatusProgress", this) as ProgressBar;
        public Slider ZoomSlider => Template.FindName("ZoomSlider", this) as Slider;
    }
}
