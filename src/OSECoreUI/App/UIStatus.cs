using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace OSECoreUI.App
{
    public class UIStatus : IUIStatus, INotifyPropertyChanged
    {
        private int _totalSteps = 0;
        private int _currentStep = 0;
        private string _status = "";
        private bool _inProgress = false;
        public string Status
        {
            get => _status;
            set
            {
                if(_status != value)
                {
                    _status = value;
                    OnPropertyChanged();
                }
            }
        }
        public int TotalSteps
        {
            get => _totalSteps;
            set
            {
                int newValue = Math.Max(0, value);
                if(newValue != _totalSteps)
                {
                    _totalSteps = value;
                    OnPropertyChanged();
                }
            }
        }
        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                int newValue = Math.Min(Math.Max(0, value), TotalSteps);
                if(_inProgress && newValue != _currentStep)
                {
                    _currentStep = newValue;
                    OnPropertyChanged();
                }
            }
        }

        public bool InProgress
        {
            get => _inProgress;
            private set
            {
                if(_inProgress != value)
                {
                    _inProgress = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NextStep(int steps = 1)
        {
            CurrentStep += steps;
        }

        public void StartProgress(int nsteps, string status = "")
        {
            InProgress = true;
            TotalSteps = nsteps;
            CurrentStep = 0;
            Status = status;
        }

        public void StopProgress(string status = "Ready")
        {
            CurrentStep = TotalSteps;
            Status = status;
            InProgress = false;
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
