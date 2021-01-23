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
using OSEUIControls.WPF.Events;

namespace OSEUIControls.WPF.Gallery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FileMetadata fmd = new FileMetadata();
            fmd.Title = "title";
            fmd.Subject = "subject";
            fmd.Comment = "comment";
            fmd.Copyright = "copyright";
            fmd.Author = new string[] {"Author 1", "Author 2"};
            fmd.Keywords = new string[] {"Keyword 1", "Keyword 2"};
        }

        private MainViewModel ViewModel => DataContext as MainViewModel;
        private void CanUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.UndoRedo.HasUndo;
        }

        private void Undo(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.UndoRedo.Undo();
        }

        private void CanRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ViewModel.UndoRedo.HasRedo;
        }

        private void Redo(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.UndoRedo.Redo();
        }

        private void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Delete(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ButtonPanel_OnButtonPressed(object sender, PanelButtonEventArgs args)
        {
            StatusBox.AppendText("Button '" + args.ButtonTag + "' pressed.\n");
            switch (args.ButtonTag as string)
            {
                case "1": ButtonPanel.EnableButtonCommand.Execute("a", NoOKCancel);
                    break;
                case "2":
                    ButtonPanel.DisableButtonCommand.Execute("a", NoOKCancel);
                    break;
                case "3":
                    ButtonPanel.EnableButtonCommand.Execute("b", NoOKCancel);
                    break;
                case "4":
                    ButtonPanel.DisableButtonCommand.Execute("b", NoOKCancel);
                    break;
            }
        }

        private void CanEnableButton(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void EnableButton(object sender, ExecutedRoutedEventArgs e)
        {
            ButtonPanel.EnableButtonCommand.Execute(e.Parameter, NoOKCancel);
            e.Handled = true;
        }

        private void CanDisableButton(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;

        }

        private void DisableButton(object sender, ExecutedRoutedEventArgs e)
        {
            ButtonPanel.DisableButtonCommand.Execute(e.Parameter, NoOKCancel);
            e.Handled = true;
        }

        private void CanFind(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Find(object sender, ExecutedRoutedEventArgs e)
        {
            StatusBox.AppendText("Find handler executed with parameter '" + e.Parameter + "'.\n");
            e.Handled = true;
        }
    }
}
