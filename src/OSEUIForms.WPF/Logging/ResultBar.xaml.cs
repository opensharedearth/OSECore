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
using OSECoreUI.Logging;

namespace OSEUIForms.WPF.Logging
{
    /// <summary>
    /// Interaction logic for StatusBarControl.xaml
    /// </summary>
    public partial class ResultBar : UserControl
    {
        private ResultLogForm _form = null;
        public static DependencyProperty ResultLogProperty = DependencyProperty.Register("ResultLog", typeof(ResultLog), typeof(ResultBar),
            new FrameworkPropertyMetadata(ResultLogChanged));
        public static DependencyProperty ResultLogViewProperty = DependencyProperty.Register("ResultLogView", typeof(ResultLogView), typeof(ResultBar));

        private static void ResultLogChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rb = d as ResultBar;
            var log = e.NewValue as ResultLog;
            if (rb != null && log != null)
            {
                var view = rb.ResultLogView;
                view?.Dispose();
                rb.ResultLogView = new ResultLogView(log);
            }
        }

        public ResultBar()
        {
            InitializeComponent();
        }

        public ResultLog ResultLog
        {
            get => GetValue(ResultLogProperty) as ResultLog;
            set => SetValue(ResultLogProperty, value);
        }
        public ResultLogView ResultLogView
        {
            get => GetValue(ResultLogViewProperty) as ResultLogView;
            set => SetValue(ResultLogViewProperty, value);
        }

        private void CanShowResultLogForm(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ShowResultLogForm(object sender, ExecutedRoutedEventArgs e)
        {
            if (_form != null && _form.IsVisible)
            {
                _form.Close();
                _form = null;
            }
            else
            {
                _form = new ResultLogForm(ResultLogView);
                _form.Owner = Window.GetWindow(this);
                _form.Closing += _form_Closing;
                _form.Show();
            }
        }

        private void _form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _form = null;
        }
    }
}
