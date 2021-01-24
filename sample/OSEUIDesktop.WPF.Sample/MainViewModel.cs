using OSECore.Logging;
using OSECoreUI.Undo;
using OSEUI.WPF.App;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OSEUIDesktop.WPF.Sample
{
    public class MainViewModel : INotifyPropertyChanged
    {
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
    }
}
