using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OSEUI.WPF.Graphics
{
    public class Axis : INotifyPropertyChanged
    {
        private double _min = Double.MinValue;
        private double _max = Double.MaxValue;
        private int _desiredTicks = 11;
        private int _actualTicks = 11;
        private double _tickIncrement = 0.0;


        public Axis()
        {
            _min = 0.0;
            _max = 100.0;
            _desiredTicks = 11;
            Update();
        }
        public Axis(double min, double max, int ticks = 11)
        {
            _min = min;
            _max = max;
            _desiredTicks = ticks;
            Update();
        }

        public double Max
        {
            get => _max;
            set
            {
                if (_max != value)
                {
                    _max = value;
                    Update();
                    OnPropertyChanged(nameof(Max));
                    OnPropertyChanged(nameof(Range));
                    OnPropertyChanged(nameof(ActualTicks));
                    OnPropertyChanged(nameof(TickIncrement));
                }
            }
        }
        public double Min
        {
            get => _min;
            set
            {
                if (_min != value)
                {
                    _min = value;
                    Update();
                    OnPropertyChanged(nameof(Min));
                    OnPropertyChanged(nameof(Range));
                    OnPropertyChanged(nameof(ActualTicks));
                    OnPropertyChanged(nameof(TickIncrement));
                }
            }
        }

        public int DesiredTicks
        {
            get => _desiredTicks;
            set
            {
                if (value <= 1) throw new ArgumentException("The axis must have at least 2 ticks.");
                if (_desiredTicks != value)
                {
                    _desiredTicks = value;
                    Update();
                    OnPropertyChanged(nameof(DesiredTicks));
                    OnPropertyChanged(nameof(ActualTicks));
                    OnPropertyChanged(nameof(TickIncrement));
                }
            }
        }

        public double TickIncrement => _tickIncrement;

        public double Range => _max - _min;

        public int ActualTicks => _actualTicks;

        public void Update()
        {
            double range = _max - _min;
            if (range > 0.0)
            {
                double inc = range / (_desiredTicks - 1);
                if (_desiredTicks <= 3)
                {
                    _tickIncrement = inc;
                    _actualTicks = _desiredTicks;
                }
                else
                {
                    double digits = Math.Floor(Math.Log10(inc) + 0.5);
                    _tickIncrement = Math.Pow(10.0, digits);
                    int ticks = (int)((Range / TickIncrement) + 0.5) + 1;
                    double lastTick = Min + (ticks - 2) * TickIncrement;
                    _actualTicks = Max - lastTick > TickIncrement / 2.0 ? ticks : ticks - 1;
                }
            }
            else
            {
                _tickIncrement = 0.0;
                _actualTicks = 2;
            }
        }

        public double[] GetTicks()
        {
            List<double> ticks = new List<double>();
            for (int tick = 0; tick < _actualTicks - 1; ++tick)
            {
                ticks.Add(_min + tick * _tickIncrement);
            }

            ticks.Add(Max);
            return ticks.ToArray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
