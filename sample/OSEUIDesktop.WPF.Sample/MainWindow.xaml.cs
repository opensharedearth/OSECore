using OSECoreUI.App;
using OSECoreUI.Document;
using OSECoreUI.Logging;
using OSEUI.WPF.App;
using OSEUI.WPF.Commands;
using OSEUI.WPF.Document;
using OSEUIForms.WPF.App;
using OSEUIForms.WPF.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace OSEUIDesktop.WPF.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateRecentDocumentMenu();
            UpdateView();
            UpdateTitle();
        }
        protected override void OnInitialized(EventArgs e)
        {
            DesktopApp.Instance.Settings.ApplyMainWindowBounds(this);
            base.OnInitialized(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            UpdateDocument();
            if (!DesktopDocument.Close(this, CurrentDocument))
            {
                e.Cancel = true;
            }
            else
            {
                DesktopApp.Instance.Settings.UpdateMainWindowBounds(this);
            }
            base.OnClosing(e);
        }
        private void UpdateTitle()
        {
            if (CurrentDocument != null)
            {
                Title = DesktopApp.Instance.Config.AppTitle + " - " + CurrentDocument.Title + (CurrentDocument.IsDirty ? "*" : "");
            }
            else
            {
                Title = DesktopApp.Instance.Config.AppTitle;
            }
        }
        private void UpdateRecentDocumentMenu()
        {
            int pos0 = FileMenu.Items.IndexOf(RecentFileMenuStart);
            int pos1 = FileMenu.Items.IndexOf(RecentFileMenuEnd);
            if (pos0 > 0 && pos1 > 0)
            {
                int n = pos1 - pos0 - 1;
                for (int i = 0; i < n; ++i) FileMenu.Items.RemoveAt(pos0 + 1);
            }

            RecentFiles rf = DesktopApp.Instance.Settings.RecentFiles;
            if (rf.Count == 0)
            {
                FileMenu.Items.Insert(pos1, new MenuItem()
                { Header = "(No Recent Files)", IsEnabled = false });
            }
            else
            {
                int index = 1;
                foreach (string path in rf)
                {
                    MenuItem mi = new MenuItem()
                    {
                        Header = $"{index}: {path}",
                        Command = DesktopAppCommands.OpenRecentDocument,
                        CommandParameter = index.ToString()
                    };
                    FileMenu.Items.Insert(pos1++, mi);
                }
            }
        }
        private MainViewModel ViewModel => DataContext as MainViewModel;
        private SampleDocument CurrentDocument => DesktopApp.Instance.CurrentDocument as SampleDocument;
        private void CanExitApplication(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
        private void ExitApplication(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
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
            e.CanExecute = false;
        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                IDataObject d = Clipboard.GetDataObject();
                e.CanExecute = false;
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IDataObject d = Clipboard.GetDataObject();
            }
            catch (Exception)
            {
                // ignored
            }
        }
        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

        }

        private void Delete(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void CanSelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
        }

        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void ShowAboutBoxHandler(object sender, ExecutedRoutedEventArgs e)
        {
            AboutBox form = new AboutBox();
            form.ShowDialog();
        }
        private void ShowSettingsCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            SettingsPageDefinition[] definitions = new SettingsPageDefinition[]
            {
                new SettingsPageDefinition("General", "GeneralSettings", DesktopApp.Instance.Config.Settings),
            };
            SettingsForm form = new SettingsForm(definitions);
            form.Owner = this;
            form.Title = "OSE Sample App Settings";
            form.ShowDialog();
        }
        private void CanNewDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentDocument == null;
        }

        private void NewDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (DesktopDocument.Close(this, CurrentDocument))
            {
                DesktopApp.Instance.NewDocument();
            }
            UpdateTitle();
        }
        private void CanOpenDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentDocument?.IsEmpty ?? true;
        }

        private void OpenDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            DesktopApp.Instance.CurrentDocument = DesktopDocument.Open(this, DesktopApp.Instance.DefaultDocType, MainModel.Instance.Log);
            DesktopApp.Instance.UpdateDocumentFolder();
            UpdateRecentFiles();
            UpdateRecentDocumentMenu();
            UpdateTitle();
            UpdateView();
        }
        private void UpdateRecentFiles()
        {
            if (CurrentDocument != null && !String.IsNullOrEmpty(CurrentDocument.Path))
            {
                DesktopApp.Instance.Settings.RecentFiles.Add(CurrentDocument.Path);
            }
        }
        private void UpdateView()
        {
            if (CurrentDocument != null)
            {
            }
        }
        private void CanSaveDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentDocument != null && (CurrentDocument.IsDirty || CurrentDocument.IsEmpty);
        }
        private void SaveDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateDocument();
            if (DesktopDocument.Save(this, CurrentDocument))
            {
                UpdateTitle();
                DesktopApp.Instance.UndoRedo.Clear();
            }
        }
        private void CanSaveAsDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentDocument != null;
        }

        private void SaveAsDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            UpdateDocument();
            if (DesktopDocument.SaveAs(this, CurrentDocument))
            {
                UpdateTitle();
                DesktopApp.Instance.UpdateDocumentFolder();
                DesktopApp.Instance.UndoRedo.Clear();
                UpdateRecentFiles();
                UpdateRecentDocumentMenu();
            }
        }
        private void UpdateDocument()
        {
            if (CurrentDocument != null)
            {
            }
        }
        private void CanCloseDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !CurrentDocument?.IsUnsaved ?? false;
        }

        private void CloseDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            DesktopApp.Instance.CloseDocument();
            UpdateTitle();
            UpdateView();
        }
        private void CanOpenRecentDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            if ((CurrentDocument?.IsEmpty ?? true) && e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int index))
            {
                e.CanExecute = DesktopApp.Instance.Settings.RecentFiles.Count >= index;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void OpenRecentDocumentHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter != null && int.TryParse(e.Parameter.ToString(), out int index) && index > 0 && index <= DesktopApp.Instance.Settings.RecentFiles.Count)
            {
                string path = DesktopApp.Instance.Settings.RecentFiles[index - 1];
                DesktopApp.Instance.CurrentDocument = BaseDocument.Open(path, ViewModel.ResultLog) as SampleDocument;
                DesktopApp.Instance.UpdateDocumentFolder();
                UpdateTitle();
                UpdateView();
            }

        }

        private void LogGood(object sender, RoutedEventArgs e)
        {
            ViewModel.ResultLog.LogGood("This is a good message.");
        }

        private void LogWarning(object sender, RoutedEventArgs e)
        {
            ViewModel.ResultLog.LogSuspect("This is a suspect message.");
        }

        private void LogError(object sender, RoutedEventArgs e)
        {
            ViewModel.ResultLog.LogBad("This is an bad message.");
        }
        private ResultLogForm _form = null;
        protected void CanShowResultLogForm(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected void ShowResultLogForm(object sender, ExecutedRoutedEventArgs e)
        {
            if (_form != null && _form.IsVisible)
            {
                _form.Close();
                _form = null;
            }
            else
            {
                _form = new ResultLogForm(new ResultLogView(MainViewModel.ResultLog));
                _form.Owner = Window.GetWindow(this);
                _form.Closing += _form_Closing;
                _form.Show();
            }
        }

        protected void _form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _form = null;
        }

        private void CanAddEntry(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !MainViewModel.InEdit;
        }

        private void AddEntry(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel.AddEntry();
        }

        private void CanEditEntry(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainViewModel.SelectedEntry != null && !MainViewModel.InEdit;
        }

        private void EditEntry(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel.InEdit = true;
        }

        private void CanDeleteEntry(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainViewModel.SelectedEntry != null && !MainViewModel.InEdit;
        }

        private void DeleteEntry(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel.DeleteEntry();
        }
    }
}
