using OSECore.Logging;
using OSECoreUI.App;
using OSECoreUI.Undo;
using OSEUI.WPF.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace OSEUIDesktop.WPF.Sample
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private UIZoom _uiZoom = new UIZoom();
        private UIStatus _uiStatus = new UIStatus();
        private MainModel _model = MainModel.Instance;
        public event PropertyChangedEventHandler PropertyChanged;
        public UndoRedoFrameworkView UndoRedo { get; }
        public MainViewModel()
        {
            ResultLog = new ResultLog();
            UndoRedo = new UndoRedoFrameworkView(DesktopApp.Instance.UndoRedo);
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public ResultLog _resultLog;
        public ResultLog ResultLog
        {
            get { return _resultLog; }
            set
            {
                if (_resultLog != value)
                {
                    _resultLog = value;
                    OnPropertyChanged();
                }
            }
        }

        public UIZoom UIZoom { get => _uiZoom; }
        public UIStatus UiStatus { get => _uiStatus; }
        public MainModel Model => _model;
        public JournalEntries Entries => Model.Document?.Entries ?? null;
        public JournalEntry _selectedEntry = null;
        public JournalEntry SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (_selectedEntry != value)
                {
                    _selectedEntry = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _selectedIndex = -1;
        public int SelectedIndex 
        {
            get => _selectedIndex;
            set
            {
                if(_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddEntry()
        {
            var entry = new JournalEntry();
            int index = Model.Document.Entries.Count;
            Model.Document.Entries.Add(entry);
            Model.Document.Dirty();
            CommitEdit();
            SelectedIndex = index;
            StartEdit();
            UndoRedo.PushUndo(new UndoObject(
                "add journal entry",
                new UndoContext(entry),
                (uc) =>
                {
                    var entry = uc[0] as JournalEntry;
                    int index = Model.Document.Entries.IndexOf(entry);
                    if (index >= 0)
                    {
                        Model.Document.Entries.RemoveAt(index);
                    }
                    return new UndoContext(index, entry);
                },
                (rc) =>
                {
                    Model.Document.Entries.Insert((int)rc[0], rc[1] as JournalEntry);
                }
                ));
        }
        private bool _inEdit = false;
        public bool NotInEdit => !InEdit;
        public bool InEdit
        {
            get => _inEdit;
            set
            {
                if (_inEdit != value)
                {
                    _inEdit = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NotInEdit));
                }
            }
        }

        private bool _showDocumentToolBar = true;
        public bool ShowDocumentToolBar
        {
            get => _showDocumentToolBar;
            set
            {
                if(_showDocumentToolBar != value)
                {
                    _showDocumentToolBar = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showEditToolBar = true;
        public bool ShowEditToolBar
        {
            get => _showEditToolBar;
            set
            {
                if (_showEditToolBar != value)
                {
                    _showEditToolBar = value;
                    OnPropertyChanged();
                }
            }
        }

        internal void DeleteEntry()
        {
            var entry = SelectedEntry;
            if (entry != null)
            {
                int index = _model.Document.Entries.IndexOf(entry);
                if (index >= 0)
                {
                    _model.Document.Entries.RemoveAt(index);
                    UndoRedo.PushUndo(new UndoObject(
                        "delete journal entry",
                        new UndoContext(index, entry),
                        (uc) =>
                        {
                            _model.Document.Entries.Insert((int)uc[0], uc[1] as JournalEntry);
                            return uc;
                        },
                        (rc) =>
                        {
                            _model.Document.Entries.RemoveAt((int)rc[0]);
                        }
                        ));
                }
            }
        }

        public void StartEdit()
        {
            if (!_inEdit && SelectedEntry != null)
            {
                var entry = new JournalEntry(SelectedEntry);
                SelectedEntry = entry;
                InEdit = true;
            }
        }
        public void CommitEdit()
        {
            var index = SelectedIndex;
            var entry = SelectedEntry;
            if (_inEdit && entry != null && index >= 0 && !entry.Equals(Entries[SelectedIndex]))
            {
                Entries[index] = entry;
                SelectedIndex = index;
                UndoRedo.PushUndo(new UndoObject(
                     "edit journal entry",
                     new UndoContext(SelectedIndex, new JournalEntry(entry)),
                     (uc) =>
                     {
                         var newEntry = Model.Document.Entries[index];
                         Model.Document.Entries[index] = newEntry;
                         return new UndoContext(index, newEntry);
                     },
                     (rc) =>
                     {
                         Model.Document.Entries[(int)rc[0]] = rc[1] as JournalEntry;
                     }
                     ));
                InEdit = false;
                Model.Document.Dirty();
            }
            else if(_inEdit)
            {
                CancelEdit();
            }
        }

        internal void CancelEdit()
        {
            var index = SelectedIndex;
            var entry = SelectedEntry;
            if (_inEdit && entry != null && index >= 0)
            {
                SelectedEntry = Entries[index];
                InEdit = false;
            }
        }
        public void UpdateViewModel()
        {
            OnPropertyChanged(nameof(Entries));
        }
    }
}
