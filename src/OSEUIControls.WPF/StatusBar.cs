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

namespace OSEUIControls.WPF
{
    public class StatusBar : Control
    {
        public static DependencyProperty ResultLogProperty = DependencyProperty.Register("ResultLog", typeof(ResultLog), typeof(StatusBar),
            new FrameworkPropertyMetadata(ResultLogChanged));
        static StatusBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBar), new FrameworkPropertyMetadata(typeof(StatusBar)));
        }
        private static void ResultLogChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StatusBar rb && e.NewValue is ResultLog log)
            {
                var view = rb.ResultLogView;
                view?.Dispose();
                rb.ResultLogView = new ResultLogView(log);
            }
        }
        public ResultLogView ResultLogView { get; set; }
        public bool HasZoom { get; set; } = false;
        public bool HasProgress { get; set; } = false;
        public double MinZoomLevel { get; set; } = 1.0;
        public double MaxZoomLevel { get; set; } = 5.0;
    }
}
