using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace OSEUIControls.WPF.Adorners
{
    public class PanAdorner : Adorner, INotifyPropertyChanged
    {
        private UIElement _uiElement;
        private double _boxThickness = 1.0;
        private Transform _transform;
        private Point _anchor;
        private Point _extent;
        private Rect _bound;
        private Pen _backgroundPen = null;
        private Pen _foregroundPen = null;
        private bool _panStarted = false;
        public PanAdorner(UIElement adornedElement, Transform transform) : base(adornedElement)
        {
            _uiElement = adornedElement;
            _transform = transform;
            _bound = new Rect(new Point(0, 0), _uiElement.RenderSize);
            _backgroundPen = new Pen(Brushes.Black, _boxThickness);
            _foregroundPen = new Pen(Brushes.White, _boxThickness);
            _foregroundPen.DashStyle = new DashStyle(new double[] { 4.0, 2.0 }, 0);
        }

        public bool PanStarted 
        {
            get => _panStarted;
            private set
            {
                _panStarted = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PanStarted)));
            }
        }

        public void StartPan(Point anchor)
        {
            _anchor = anchor;
            PanStarted = true;
        }

        public void UpdatePan(Point extent)
        {
            _extent = extent;
            InvalidateVisual();
        }

        public Point StopPan()
        {
            PanStarted = false;
            InvalidateVisual();
            Vector offset = _anchor - _extent;
            Point center = new Point(_bound.Width / 2.0, _bound.Height / 2.0);
            return center + offset;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (PanStarted)
            {
                Vector offset = _anchor - _extent;
                Rect box = _bound;
                box.Offset(-offset);
                drawingContext.DrawLine(_backgroundPen, new Point(_extent.X, box.Top), new Point(_extent.X, box.Bottom));
                drawingContext.DrawLine(_foregroundPen, new Point(_extent.X, box.Top), new Point(_extent.X, box.Bottom));
                drawingContext.DrawLine(_backgroundPen, new Point(box.Left, _extent.Y), new Point(box.Right, _extent.Y));
                drawingContext.DrawLine(_foregroundPen, new Point(box.Left, _extent.Y), new Point(box.Right, _extent.Y));
                drawingContext.DrawRectangle(Brushes.Transparent, _backgroundPen, box);
                drawingContext.DrawRectangle(Brushes.Transparent, _foregroundPen, box);
            }
        }
    }
}
