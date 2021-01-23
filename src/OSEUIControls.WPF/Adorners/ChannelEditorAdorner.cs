using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using OSECoreUI.Annotations;
using OSEUI.WPF.Graphics;

namespace OSEUIControls.WPF.Adorners
{
    public class ChannelEditorAdorner : Adorner, INotifyPropertyChanged
    {
        private IPointSeries _points = new PointSeries();
        private IPointSeries _originalPoints;
        private IPointSeries _selectedPoints = new PointSeries();
        public IPointSeries SelectedPoints
        {
            get => _selectedPoints;
            set
            {
                if (_selectedPoints != value && value != null)
                {
                    _selectedPoints = value;
                    InvalidateVisual();
                }
            }
         }

        private double _pointSize = 2.0;
        private double _lineThickness = 2.0;
        private double _selectionBoxThickness = 1.0;
        private Color _color = Colors.Black;
        private Canvas _editor = null;
        private Brush _brush = null;
        private Pen _pen = null;
        private Pen _selectionPen = null;
        private Point? _start = null;
        private Point _mid = new Point();
        private Point? _end = null;
        private Point _anchor;
        private Point _extent;
        private bool _inEdit = false;
        private bool _inSelection = false;
        private Rect _boundary;
        private Transform _transform;
        public ChannelEditorAdorner(Canvas editor, Color color, double pointSize, double lineThickness, Transform transform)
        : base(editor)
        {
            _editor = editor;
            _color = color;
            _pointSize = pointSize;
            _lineThickness = lineThickness;
            _brush = new SolidColorBrush(_color);
            _pen = new Pen(_brush, _lineThickness);
            _selectionPen = new Pen(SystemColors.HighlightBrush, _selectionBoxThickness);
            _transform = transform;
            IsHitTestVisible = false;
        }

        public bool InEdit => _inEdit;
        public bool InSelection => _inSelection;

        public PointSeries GetEditedPoints()
        {
            return new PointSeries(_points);
        }
        public PointSeries GetOriginalPoints()
        {
            return new PointSeries(_originalPoints);
        }
        public int Index { get; private set; }
        public double Value { get; private set; }

        public void Clear()
        {
            _inEdit = false;
            _inSelection = false;
            _points.Clear();
            InvalidateVisual();
        }

        public void StartSelection(Point anchor)
        {
            Clear();
            if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl))
            {
                ClearSelection();
            }
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

        public void ClearSelection()
        {
            _selectedPoints.Clear();
            InvalidateVisual();
        }
        public Rect GetSelectionBox()
        {
            return new Rect(_anchor, _extent);
        }
        public void StartEdit(Point? start, Point mid, Point? end, Rect boundary)
        {
            ClearSelection();
            _start = start;
            _mid = mid;
            _end = end;
            _inEdit = true;
            _boundary = boundary;
            _points.Clear();
            if (_start != null) _points.Add((Point) _start);
            _points.Add(_mid);
            if (_end != null) _points.Add((Point) _end);
            _originalPoints = new PointSeries(_points);
            InvalidateVisual();
            Index = (int) _mid.X;
            Value = (double) _mid.Y;
        }

        public void UpdateEdit(Point mid)
        {
            _mid = new Point(Math.Max(_boundary.Left, Math.Min(_boundary.Right, mid.X)),
                Math.Max(_boundary.Top, Math.Min(_boundary.Bottom, mid.Y)));
            _points.Clear();
            if (_start != null) _points.Add((Point)_start);
            _points.Add(_mid);
            if (_end != null) _points.Add((Point)_end);
            InvalidateVisual();
            Index = (int)_mid.X;
            Value = (double)_mid.Y;
            OnPropertyChanged(nameof(Index));
            OnPropertyChanged(nameof(Value));
        }

        public void EndEdit()
        {
            _inEdit = false;
            _points.Clear();
            InvalidateVisual();
        }


        public void Add(Point p)
        {
            _points.Add(p);
            InvalidateVisual();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            Point? last = null;
            foreach (Point p in _points)
            {
                Point p1 = _transform.Transform(p);
                drawingContext.DrawEllipse(_brush, null, p1, _pointSize / 2.0, _pointSize / 2.0);
                if (last != null)
                {
                    Point p0 = (Point) last;
                    drawingContext.DrawLine(_pen, p0, p1);
                }

                last = p1;
            }

            if (_inSelection)
            {
                Point anchor0 = _transform.Transform(_anchor);
                Point extent0 = _transform.Transform(_extent);
                drawingContext.DrawRectangle(Brushes.Transparent, _selectionPen, new Rect(anchor0, extent0));
            }

            if (SelectedPoints != null)
            {
                foreach (Point p in SelectedPoints)
                {
                    Point p1 = _transform.Transform(p);
                    drawingContext.DrawEllipse(_brush, null, p1, _pointSize / 2.0, _pointSize / 2.0);
                }
            }
           
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
