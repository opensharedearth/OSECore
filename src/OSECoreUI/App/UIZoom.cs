using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace OSECoreUI.App
{
    public class UIZoom : INotifyPropertyChanged
    {
        double _minZoomLevel = 1.0;
        double _maxZoomLevel = 5.0;
        double _zoomStep = 0.25;
        double _zoomLevel = 1.0;

        public double MinZoomLevel
        {
            get => _minZoomLevel;
            set
            {
                if(value > 0.0 && value != _minZoomLevel)
                {
                    _minZoomLevel = value;
                    OnPropertyChanged();
                }
            }
        }
        public double MaxZoomLevel 
        { 
            get => _maxZoomLevel;
            set
            {
                if(value > 0.0 && value != _maxZoomLevel)
                {
                    _maxZoomLevel = value;
                    OnPropertyChanged();
                }
            }
        }
        public double ZoomStep 
        { 
            get => _zoomStep;
            set
            {
                if(value > 0.0 && value != _zoomStep)
                {
                    _zoomStep = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ZoomLevel 
        {
            get => _zoomLevel;
            set
            {
                if(value >= _minZoomLevel && value <= _minZoomLevel && value != _zoomLevel)
                {
                    _zoomLevel = value;
                    OnPropertyChanged();
                }
            }
        }
        public void NextStep(int step = 1)
        {
            ZoomLevel += step * ZoomStep; 
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
