using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OSECoreUI.Undo;

namespace OSEUIControls.WPF.Gallery
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private double _min = 0.0;
        private double _max = 100.0;
        private int _desiredTicks = 10;
        private int _actualTicks = 0;
        private double _tickIncrement = 0.0;
        public UndoRedoFrameworkView UndoRedo { get; } = new UndoRedoFrameworkView(App.UndoRedo);


        public double Min
        {
            get { return _min; }
            set
            {
                if (_min != value)
                {
                    _min = value;
                    OnPropertyChanged(nameof(Min));
                }
            }
        }

        public double Max
        {
            get { return _max; }
            set
            {
                if (_max != value)
                {
                    _max = value;
                    OnPropertyChanged(nameof(Max));
                }
            }
        }

        public int DesiredTicks
        {
            get { return _desiredTicks; }
            set
            {
                if (_desiredTicks != value)
                {
                    _desiredTicks = value;
                    OnPropertyChanged(nameof(DesiredTicks));
                }
            }
        }

        public int ActualTicks
        {
            get { return _actualTicks; }
            set
            {
                if (_actualTicks != value)
                {
                    _actualTicks = value;
                    OnPropertyChanged(nameof(ActualTicks));
                }
            }
        }

        public double TickIncrement
        {
            get { return _tickIncrement; }
            set
            {
                if (_tickIncrement != value)
                {
                    _tickIncrement = value;
                    OnPropertyChanged(nameof(TickIncrement));
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
