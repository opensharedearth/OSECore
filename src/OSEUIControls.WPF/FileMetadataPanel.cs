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
using OSECoreUI.IO;

namespace OSEUIControls.WPF
{
    public class FileMetadataPanel : Control
    {
        static FileMetadataPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FileMetadataPanel), new FrameworkPropertyMetadata(typeof(FileMetadataPanel)));
        }
        public static readonly DependencyProperty IsMetadataSupportedProperty = DependencyProperty.Register("IsMetadataSupported", typeof(bool), typeof(FileMetadataPanel), new PropertyMetadata(true, new PropertyChangedCallback(IsMetadataSupportedChanged)));

        private static void IsMetadataSupportedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FileMetadataPanel fmdp && fmdp.Template != null && e.NewValue is bool b)
            {
                if (fmdp.Template.FindName("NotSupportedLabel", fmdp) is Label label) label.Visibility = b ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public static readonly DependencyProperty FileMetadataProperty = DependencyProperty.Register("FileMetadata", typeof(FileMetadata), typeof(FileMetadataPanel), new PropertyMetadata(new FileMetadata(), new PropertyChangedCallback(FileMetadataChanged)));

        private static void FileMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public override void OnApplyTemplate()
        {
            if (Template.FindName("NotSupportedLabel", this) is Label label) label.Visibility = IsMetadataSupported ? Visibility.Hidden : Visibility.Visible;
            base.OnApplyTemplate();
        }

        public bool IsMetadataSupported
        {
            get => (bool)GetValue(IsMetadataSupportedProperty);
            set => SetValue(IsMetadataSupportedProperty, value);
        }

        public FileMetadata FileMetadata
        {
            get => GetValue(FileMetadataProperty) as FileMetadata;
            set => SetValue(FileMetadataProperty, value);
        }
    }
}
