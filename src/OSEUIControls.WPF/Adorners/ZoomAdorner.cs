using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using OSECoreUI.Annotations;

namespace OSEUI.WPF.Graphics
{
    public class ZoomAdorner : Adorner, INotifyPropertyChanged
    {
        private UIElement _uiElement;
        private double _selectionBoxThickness = 1.0;
        private Point _anchor;
        private Point _extent;
        private Transform _transform;
        private bool _inSelection = false;
        private Pen _backgroundPen = null;
        private Pen _foregroundPen = null;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool InSelection => _inSelection;

        public Point Anchor => _anchor;

        public Point Extent => _extent;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ZoomAdorner([NotNull] UIElement adornedElement, Transform transform) : base(adornedElement)
        {
            _uiElement = adornedElement;
            _transform = transform;
            _backgroundPen = new Pen(Brushes.Black, _selectionBoxThickness);
            _foregroundPen = new Pen(Brushes.White, _selectionBoxThickness);
            _foregroundPen.DashStyle = new DashStyle(new double[] { 4.0, 2.0 }, 0);
            IsHitTestVisible = false;
        }
        public void Clear()
        {
            _inSelection = false;
            InvalidateVisual();
        }
        public void StartSelection(Point anchor)
        {
            Clear();
            _inSelection = true;
            _anchor = _extent = anchor;
        }
        public void UpdateSelection(Point extent)
        {
            _extent = extent;
            InvalidateVisual();
        }
        public void EndSelection()
        {
            _inSelection = false;
            InvalidateVisual();
        }
        public Rect GetSelectionBox()
        {
            return new Rect(_anchor, _extent);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_inSelection)
            {
                Point anchor0 = _transform.Transform(_anchor);
                Point extent0 = _transform.Transform(_extent);
                drawingContext.DrawRectangle(Brushes.Transparent, _backgroundPen, new Rect(anchor0, extent0));
                drawingContext.DrawRectangle(Brushes.Transparent, _foregroundPen, new Rect(anchor0, extent0));
            }
        }
    }
}