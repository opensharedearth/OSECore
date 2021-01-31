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
        public JournalEntries Entries => Model.Document.Entries;
        public JournalEntry _selectedEntry = null;
        public JournalEntry SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if(_selectedEntry != value)
                {
                    _selectedEntry = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddEntry()
        {
            var entry = new JournalEntry();
            Model.Document.Entries.Add(entry);
            _selectedEntry = entry;
            UndoRedo.PushUndo(new UndoObject(
                "add journal entry",
                new UndoContext(entry),
                (uc) =>
                {
                    var entry = uc[0] as JournalEntry;
                    int index = Model.Document.Entries.IndexOf(entry);
                    if(index >= 0)
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
                if(_inEdit != value)
                {
                    var entry = SelectedEntry;
                    int index = Model.Document.Entries.IndexOf(entry);
                    if (entry != null && index >= 0)
                    {
                        _inEdit = value;
                        if(_inEdit)
                        {
                            UndoRedo.StartSequence();
                            UndoRedo.PushUndo(new UndoObject(
                                "edit journal entry",
                                new UndoContext(index, new JournalEntry(entry)),
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
                        }
                        else
                        {
                            UndoRedo.EndSequence("edit journal entry");
                        }
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal void DeleteEntry()
        {
            var entry = SelectedEntry;
            if(entry != null)
            {
                int index = _model.Document.Entries.IndexOf(entry);
                if(index >= 0)
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
    }
}
