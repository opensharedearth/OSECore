using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace OSEUIForms.WPF.App    
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        public AboutBox()
        {
            InitializeComponent();
            DataContext = this;
            Title = "About " + ProductName;
        }
        public string ProductName
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                AssemblyProductAttribute att = a.GetCustomAttribute<AssemblyProductAttribute>();
                return att != null ? att.Product : "";
            }
        }
        public string CompanyName
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                AssemblyCompanyAttribute att = a.GetCustomAttribute<AssemblyCompanyAttribute>();
                return att != null ? att.Company : "";
            }
        }
        public string ProductDescription
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                AssemblyDescriptionAttribute att = a.GetCustomAttribute<AssemblyDescriptionAttribute>();
                return att != null ? att.Description : "";
            }
        }
        public string ProductCopyright
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                AssemblyCopyrightAttribute att = a.GetCustomAttribute<AssemblyCopyrightAttribute>();
                return att != null ? att.Copyright : "";
            }
        }
        public string ProductVersion
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                return a.GetName().Version.ToString();
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public BitmapSource Image
        {
            get
            {
                Assembly a = Assembly.GetEntryAssembly();
                AboutBoxImageAttribute att = a.GetCustomAttribute<AboutBoxImageAttribute>();
                if (att != null)
                {
                    var assembly = System.Reflection.Assembly.GetEntryAssembly();
                    var bitmap = new BitmapImage();
                    //using (var stream =
                    //    assembly.GetManifestResourceStream(att.Path))
                    //{
                    //    if (stream != null)
                    //    {
                    //        bitmap.BeginInit();
                    //        bitmap.StreamSource = stream;
                    //        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    //        bitmap.EndInit();
                    //    }
                    //}
                    StreamResourceInfo sri = Application.GetResourceStream(new Uri(att.Path, UriKind.Relative));
                    bitmap.BeginInit();
                    bitmap.StreamSource = sri.Stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }

                return null;
            }
        }
    }
}
