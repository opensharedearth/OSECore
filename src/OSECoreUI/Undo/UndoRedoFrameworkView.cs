using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OSECore.Logging;

namespace OSECoreUI.Undo
{
    public class UndoRedoFrameworkView : INotifyPropertyChanged, IUndoRedo
    {
        UndoRedoFramework _framework = null;
        public UndoRedoFrameworkView(UndoRedoFramework framework)
        {
            _framework = framework;
        }
        public Result Undo()
        {
            Result r = _framework.Undo();
            OnPropertyChanged(nameof(RedoTitle));
            OnPropertyChanged(nameof(UndoTitle));
            return r;
        }

        public Result Redo()
        {
            Result r = _framework.Redo();
            OnPropertyChanged(nameof(RedoTitle));
            OnPropertyChanged(nameof(UndoTitle));
            return r;
        }
        public void PushUndo(UndoObject uo)
        {
            if (_framework.IsEnabled)
            {
                _framework.PushUndo(uo);
                OnPropertyChanged(nameof(RedoTitle));
                OnPropertyChanged(nameof(UndoTitle));
            }
        }
        public void StartSequence()
        {
            _framework.StartSequence();
        }
        public UndoSequence EndSequence(string title, bool reverse = false)
        {
            UndoSequence uso = _framework.EndSequence(title, reverse);
            OnPropertyChanged(nameof(UndoTitle));
            return uso;
        }
        public string UndoTitle
        {
            get
            {
                return _framework.UndoTitle;
            }
        }
        public string RedoTitle
        {
            get
            {
                return _framework.RedoTitle;
            }
        }
        public bool HasUndo
        {
            get
            {
                return _framework.HasUndo;
            }
        }
        public bool HasRedo
        {
            get
            {
                return _framework.HasRedo;
            }
        }

        public UndoRedoFramework UndoRedo
        {
            get => _framework;
        }

        public void Clear()
        {
            _framework.Clear();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
