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
using System.Windows.Shapes;

namespace OSEUIForms.WPF.App
{
    /// <summary>
    /// Interaction logic for SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm : Window
    {
        private SettingsPageDefinition[] _definitions;
        public SettingsPageDefinition[] Definitions => _definitions;
        public SettingsForm(SettingsPageDefinition[] definitions = null)
        {
            InitializeComponent();
            _definitions = definitions;
            DataContext = this;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
