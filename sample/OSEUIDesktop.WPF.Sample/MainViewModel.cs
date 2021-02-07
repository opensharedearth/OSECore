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
            UiStatus.StopProgress();
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

        public SampleSettings Settings => App.Instance.Settings;
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
        public JournalEntry _entryInEdit = null;
        public JournalEntry EntryInEdit
        {
            get => _entryInEdit;
            set
            {
                if(_entryInEdit != value)
                {
                    _entryInEdit = value;
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
                new UndoContext(index, entry),
                (uc) =>
                {
                    var index = (int)uc[0];
                    var entry = uc[1] as JournalEntry;
                    CancelEdit();
                    Model.Document.Entries.RemoveAt(index);
                    return new UndoContext(index, entry);
                },
                (rc) =>
                {
                    var index = (int)rc[0];
                    var entry = rc[1] as JournalEntry;
                    CancelEdit();
                    Model.Document.Entries.Insert(index,entry);
                    SelectedIndex = index;
                    StartEdit();
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
            var index = SelectedIndex;
            if(index >= 0 && index < _model.Document.Entries.Count)
            {
                var entry = _model.Document.Entries[index];
                CancelEdit();
                _model.Document.Entries.RemoveAt(index);
                _model.Document.Dirty();
                UndoRedo.PushUndo(new UndoObject(
                    "delete journal entry",
                    new UndoContext(index, entry),
                    (uc) =>
                    {
                        CancelEdit();
                        _model.Document.Entries.Insert((int)uc[0], uc[1] as JournalEntry);
                        return uc;
                    },
                    (rc) =>
                    {
                        CancelEdit();
                        _model.Document.Entries.RemoveAt((int)rc[0]);
                    }
                    ));
            }
        }

        public void StartEdit()
        {
            if (!_inEdit && SelectedEntry != null)
            {
                EntryInEdit = new JournalEntry(SelectedEntry);
                InEdit = true;
            }
        }
        public void CommitEdit()
        {
            var index = SelectedIndex;
            var entry = EntryInEdit;
            var entry0 = SelectedEntry;
            if (_inEdit && entry != null && index >= 0 && !entry.Equals(entry0))
            {
                Entries[index] = entry;
                SelectedIndex = index;
                UndoRedo.PushUndo(new UndoObject(
                     "edit journal entry",
                     new UndoContext(index, new JournalEntry(entry0)),
                     (uc) =>
                     {
                         int index = (int)uc[0];
                         var entry = uc[1] as JournalEntry;
                         CancelEdit();
                         var newEntry = Model.Document.Entries[index];
                         Model.Document.Entries[index] = entry;
                         SelectedEntry = entry;
                         return new UndoContext(index, newEntry);
                     },
                     (rc) =>
                     {
                         CancelEdit();
                         int index = (int)rc[0];
                         var entry = rc[1] as JournalEntry;
                         Model.Document.Entries[index] = entry;
                         SelectedEntry = entry;
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
            if (_inEdit)
            {
                EntryInEdit = null;
                InEdit = false;
            }
        }
        public void UpdateViewModel()
        {
            OnPropertyChanged(nameof(Entries));
        }
    }
}
