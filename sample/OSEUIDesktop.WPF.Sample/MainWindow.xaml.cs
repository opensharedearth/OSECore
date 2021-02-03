using Microsoft.Win32;
using OSECoreUI.App;
using OSECoreUI.Document;
using OSECoreUI.Logging;
using OSEUI.WPF.App;
using OSEUI.WPF.Commands;
using OSEUI.WPF.Document;
using OSEUIControls.WPF;
using OSEUIForms.WPF.App;
using OSEUIForms.WPF.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "InEdit" && ViewModel.InEdit)
            {
                EntryTitle.Focus();
            }
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
            MainModel.Instance.Document = CurrentDocument as SampleDocument;
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
            MainModel.Instance.Document = CurrentDocument as SampleDocument;
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
            MainViewModel.UpdateViewModel();
            if (CurrentDocument != null)
            {
                if(MainViewModel.SelectedIndex < 0 && CurrentDocument.Entries.Count > 0)
                {
                    MainViewModel.SelectedIndex = 0;
                }
            }
            else
            {
                MainViewModel.SelectedIndex = -1;
            }
        }
        private void CanSaveDocument(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CurrentDocument != null && CurrentDocument.IsDirty;
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
            MainModel.Instance.Document = null;
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
                MainModel.Instance.Document = CurrentDocument as SampleDocument;
                UpdateView();
            }

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
            MainViewModel.StartEdit();
        }

        private void CanDeleteEntry(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainViewModel.SelectedEntry != null && !MainViewModel.InEdit;
        }

        private void DeleteEntry(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel.DeleteEntry();
        }

        private void EntryListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                MainViewModel.SelectedEntry = e.AddedItems[0] as JournalEntry;
            }
            else
            {
                MainViewModel.SelectedEntry = null;
            }
        }

        private void EditFormButtonPressed(object sender, OSEUIControls.WPF.Events.PanelButtonEventArgs args)
        {
            if(args.ButtonTag == ButtonPanel.OKTag)
            {
                if (EntryTitle.IsFocused) EntryTitle.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                MainViewModel.CommitEdit();
            }
            else if(args.ButtonTag == ButtonPanel.CancelTag)
            {
                MainViewModel.CancelEdit();
            }
        }

        private void CanAddImage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainViewModel.SelectedEntry != null && MainViewModel.SelectedEntry.Image == null;
        }

        private void AddImage(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = App.Instance.Settings.DocumentFolder;
            dialog.Filter = "Images {*.gif, *.jpg, *.png, *.bmp)|*.gif;*.jpg;*.jpeg;*.png;*.bmp|All Files (*.*)|*.*";
            if(dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                var bitmap = new BitmapImage(new Uri(path, UriKind.Absolute));
                MainViewModel.SelectedEntry.Image = bitmap;
            }
        }

        private void CanDeleteImage(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = MainViewModel.SelectedEntry != null && MainViewModel.SelectedEntry.Image != null;
        }

        private void DeleteImage(object sender, ExecutedRoutedEventArgs e)
        {
            MainViewModel.SelectedEntry.Image = null;
        }

        private void OnTitleFocused(object sender, KeyboardFocusChangedEventArgs e)
        {
            EntryTitle.CaretIndex = EntryTitle.Text.Length;
        }

        private void OnContentFocused(object sender, KeyboardFocusChangedEventArgs e)
        {
            EntryContent.CaretIndex = EntryTitle.Text.Length;
        }
    }
}
