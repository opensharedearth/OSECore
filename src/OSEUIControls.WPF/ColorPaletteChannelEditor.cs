using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSECore.Logging;
using OSECoreUI.Colors;
using OSECoreUI.Graphics;
using OSECoreUI.Undo;
using OSEUI.WPF.Colors;
using OSEUI.WPF.Graphics;
using OSEUIControls.WPF.Adorners;
using Range = OSECoreUI.Graphics.Range;

namespace OSEUIControls.WPF
{
    public class ColorPaletteChannelEditor : Control, IUndoable
    {
        private const double MinLineThickness = 2.0;
        private const double MinPointSize = 2.0;
        private int _minWidth = 50;
        private int _minHeight = 100;
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register("Palette", typeof(IColorPalette), typeof(ColorPaletteChannelEditor),
            new PropertyMetadata(null, new PropertyChangedCallback(OnPaletteChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ColorPaletteChannelEditor));
        public static readonly DependencyProperty ChannelProperty =
            DependencyProperty.Register("Channel", typeof(ColorChannel), typeof(ColorPaletteChannelEditor));

        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register("LineThickness", typeof(double), typeof(ColorPaletteChannelEditor),
                new PropertyMetadata(4.0), new ValidateValueCallback(IsLineThicknessValid)
            );
        public static readonly DependencyProperty PointSizeProperty = DependencyProperty.Register("PointSize", typeof(double), typeof(ColorPaletteChannelEditor),
            new PropertyMetadata(10.0), new ValidateValueCallback(IsPointSizeValid));
        public static readonly DependencyProperty CurveFittingToleranceProperty = DependencyProperty.Register("CurveFittingTolerance", typeof(float), typeof(ColorPaletteChannelEditor),
            new PropertyMetadata(0.5e-2f),
            new ValidateValueCallback(IsToleranceValid));
        public static readonly DependencyProperty UndoRedoProperty = DependencyProperty.Register("UndoRedo", typeof(IUndoRedo), typeof(ColorPaletteChannelEditor),
            new PropertyMetadata(new UndoRedoFramework()));
        ChannelEditorAdorner _adorner = null;


        static ColorPaletteChannelEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPaletteChannelEditor), new FrameworkPropertyMetadata(typeof(ColorPaletteChannelEditor)));

        }


        public ColorPaletteChannelEditor()
        {
            PreviewMouseMove += ColorPaletteChannelEditor_PreviewMouseMove;
            MouseLeave += ColorPaletteChannelEditor_MouseLeave;
            MouseLeftButtonDown += ColorPaletteChannelEditor_MouseLeftButtonDown;
            MouseLeftButtonUp += ColorPaletteChannelEditor_MouseLeftButtonUp;
            PreviewKeyDown += ColorPaletteChannelEditor_PreviewKeyDown;
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Delete, new ExecutedRoutedEventHandler(Delete), new CanExecuteRoutedEventHandler(CanDelete)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(Cut), new CanExecuteRoutedEventHandler(CanCut)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(Copy), new CanExecuteRoutedEventHandler(CanCopy)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(Paste), new CanExecuteRoutedEventHandler(CanPaste)));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, new ExecutedRoutedEventHandler(SelectAll), new CanExecuteRoutedEventHandler(CanSelectAll)));
        }

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PointSeriesConverter.ClipboardDataAvialable();
        }

        private void CanSelectAll(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _adorner != null;
        }

        private void SelectAll(object sender, ExecutedRoutedEventArgs e)
        {
            _adorner.SelectedPoints = new PointSeries(_controlPoints);
            InvalidateVisual();
        }

        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {
            PointSeries ps = PointSeriesConverter.GetClipboard();
            if (ps != null)
            {
                IPointSeries pp1 = PointSeries.Union(_controlPoints, ps);
                UndoRedo.PushUndo(new UndoObject(
                    "paste " + Channel.ToString() + " points.",
                    new UndoContext(new PointSeries(_controlPoints), new PointSeries(pp1)),
                    (uc) => {
                        Palette.UpdateControlPoints(Channel, uc[0] as PointSeries);
                        InvalidateVisual();
                        return uc;
                    },
                    (rc) =>
                    {
                        Palette.UpdateControlPoints(Channel, rc[1] as PointSeries);
                        InvalidateVisual();
                    }));
                Palette.UpdateControlPoints(Channel, pp1);
                _controlPoints = Palette.GetControlPoints(Channel);
                InvalidateVisual();
                _adorner.ClearSelection();
            }
        }

        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _adorner != null && _adorner.SelectedPoints.Count > 0;
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
            CopySelection();
        }

        private void CopySelection()
        {
            PointSeriesConverter.SetClipboard(_adorner.SelectedPoints);
        }

        private void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _adorner != null && _adorner.SelectedPoints.Count > 0;
        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {
            UndoRedo.StartSequence();
            CopySelection();
            DeleteSelection();
            UndoRedo.EndSequence("cut " + Channel.ToString() + " points.");
        }

        private void Delete(object sender, ExecutedRoutedEventArgs e)
        {
            DeleteSelection();
        }

        private void DeleteSelection()
        {
            IPointSeries plp = PointSeries.Union(_controlPoints.EndPoints,
                PointSeries.Difference(_controlPoints, _adorner.SelectedPoints));
            UndoRedo.PushUndo(new UndoObject(
                "delete " + Channel.ToString() + " point deletion.",
                new UndoContext(new PointSeries(_controlPoints), new PointSeries(plp)),
                (uc) => { Palette.UpdateControlPoints(Channel, uc[0] as PointSeries);
                    InvalidateVisual();
                    return uc;
                },
                (rc) =>
                {
                    Palette.UpdateControlPoints(Channel, rc[1] as PointSeries);
                    InvalidateVisual();
                }));
            Palette.UpdateControlPoints(Channel, plp);
            _controlPoints = Palette.GetControlPoints(Channel);
            _adorner.ClearSelection();
            InvalidateVisual();
        }

        private void CanDelete(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _adorner != null && _adorner.SelectedPoints.Count > 0;
        }

        private void ColorPaletteChannelEditor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_adorner != null)
            {
                if (_adorner.InEdit)
                {
                    PointSeries pp1 = _adorner.GetEditedPoints();
                    PointSeries pp0 = _adorner.GetOriginalPoints();
                    _adorner.EndEdit();
                    ReleaseMouseCapture();
                    Palette.UpdateControlPoints(Channel, pp1);
                    e.Handled = true;
                    Tracker.IsOpen = false;
                    UndoRedo.PushUndo(new UndoObject(
                        "edit " + Channel.ToString() + " channel.",
                        new UndoContext(new PointSeries(pp0), new PointSeries(pp1)),
                        (uc) =>
                        {
                            Palette.UpdateControlPoints(Channel, uc[0] as PointSeries);
                            InvalidateVisual();
                            return uc;
                        },
                        (rc) =>
                        {
                            Palette.UpdateControlPoints(Channel, rc[1] as PointSeries);
                            InvalidateVisual();
                        }));
                }
                else if (_adorner.InSelection)
                {
                    _adorner.EndSelection();
                    ReleaseMouseCapture();
                }
            }
        }
        private void ColorPaletteChannelEditor_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _adorner != null)
            {
                if (_adorner.InEdit)
                {
                    _adorner.EndEdit();
                    ReleaseMouseCapture();
                    Tracker.IsOpen = false;
                    e.Handled = true;
                }
            }
        }

        private void ColorPaletteChannelEditor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_adorner != null)
            {

                Point p = e.GetPosition(Canvas);
                Point? p1p = GetLinePoint(Canvas, p);
                int index = GetControlPoint(Canvas, p, _controlPoints);
                if (index >= 0)
                {
                    if (index == 0)
                    {
                        Rect boundary = new Rect(0.0, 0.0, 0.0, 1.0);
                        Point pstart = _controlPoints[index];
                        Point pend = _controlPoints[index + 1];
                        _adorner.StartEdit(null, pstart, pend, boundary);

                    }
                    else if (index == _controlPoints.Count - 1)
                    {
                        Rect boundary = new Rect(Palette.Size - 1, 0.0, 0.0, 1.0);
                        _adorner.StartEdit(_controlPoints[index - 1], _controlPoints[index], null,
                            boundary);

                    }
                    else
                    {
                        Point p0 = _controlPoints[index - 1];
                        Point p1 = _controlPoints[index + 1];
                        Rect boundary = new Rect(p0.X + 1.0, 0.0, p1.X - p0.X - 2.0, 1.0);
                        _adorner.StartEdit(_controlPoints[index - 1], _controlPoints[index], _controlPoints[index + 1],
                            boundary);
                    }


                    e.Handled = true;
                }
                else if (p1p != null)
                {
                    Point pmid = (Point)p1p;
                    for (int i = 1; i < _controlPoints.Count; ++i)
                    {
                        if (_controlPoints[i].X >= pmid.X)
                        {
                            Point pstart = _controlPoints[i - 1];
                            Point pend = _controlPoints[i];
                            Rect boundary = new Rect(pstart.X + 1.0, 0.0, Math.Max(0.0, pend.X - pstart.X - 2.0), 1.0);
                            _adorner.StartEdit(pstart, pmid, pend, boundary);
                            e.Handled = true;
                            break;
                        }
                    }

                }
                else
                {
                    _adorner.StartSelection(_inverse.Transform(p));
                }
                if (_adorner.InEdit)
                {
                    Popup popup = Tracker;
                    popup.PlacementTarget = this;
                    popup.Placement = PlacementMode.RelativePoint;
                    popup.HorizontalOffset = p.X + 30;
                    popup.VerticalOffset = p.Y + 30;
                    popup.DataContext = _adorner;
                    popup.IsOpen = true;
                    CaptureMouse();
                    Keyboard.Focus(this);
                }
                else if (_adorner.InSelection)
                {
                    CaptureMouse();
                    Keyboard.Focus(this);

                }
            }
        }

        private int GetControlPoint(Canvas canvas, Point p, IPointSeries pp0)
        {
            List<Rect> rr = new List<Rect>();
            VisualTreeHelper.HitTest(Canvas,
                o => o is Ellipse
                    ? HitTestFilterBehavior.Continue
                    : HitTestFilterBehavior.ContinueSkipSelf,
                result =>
                {
                    if (result.VisualHit is Ellipse e)
                    {
                        Rect r = new Rect(Canvas.GetLeft(e), Canvas.GetTop(e), e.Width, e.Height);
                        rr.Add(r);
                    }
                    return HitTestResultBehavior.Continue;
                }, new PointHitTestParameters(p));
            foreach (Rect r1 in rr)
            {
                for (int i = 0; i < pp0.Count; ++i)
                {
                    Point p0 = _transform.Transform(pp0[i]);
                    if (r1.Contains(p0))
                    {
                        return i;
                    }
                }

            }

            return -1;
        }
        private Point? GetLinePoint(Canvas canvas, Point p)
        {
            Point? p1 = null;
            VisualTreeHelper.HitTest(Canvas,
                o => o is Line
                    ? HitTestFilterBehavior.Continue
                    : HitTestFilterBehavior.ContinueSkipSelf,
                result =>
                {
                    if (result.VisualHit is Line l)
                    {
                        double x = p.X;
                        double y = l.Y2 + ((l.X2 - x) / (l.X2 - l.X1)) * (l.Y1 - l.Y2);
                        p1 = _inverse.Transform(new Point(x, y));
                    }
                    return HitTestResultBehavior.Continue;
                }, new PointHitTestParameters(p));
            return p1;
        }


        private void ColorPaletteChannelEditor_MouseLeave(object sender, MouseEventArgs e)
        {
            _adorner?.Clear();
        }

        private void ColorPaletteChannelEditor_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_adorner != null)
            {
                if (_adorner.InEdit)
                {
                    Point p0 = e.GetPosition(Canvas);
                    Point p1 = _inverse.Transform(p0);
                    Tracker.HorizontalOffset = p0.X + 30;
                    Tracker.VerticalOffset = p0.Y + 30;
                    _adorner.UpdateEdit(p1);

                }
                else if (_adorner.InSelection)
                {
                    Point p0 = e.GetPosition(Canvas);
                    Point p1 = _inverse.Transform(p0);
                    _adorner.UpdateSelection(p1);
                    Rect box = _adorner.GetSelectionBox();
                    _adorner.SelectedPoints = _controlPoints.SubSet(box);

                }
                else
                {
                    Point p = e.GetPosition(Canvas);
                    int index = GetControlPoint(Canvas, p, _controlPoints);
                    if (index >= 0)
                    {
                        _adorner.Add(_controlPoints[index]);
                    }
                    else
                    {
                        _adorner?.Clear();
                        Point? p1 = GetLinePoint(Canvas, p);
                        if (p1 != null)
                        {
                            _adorner.Add((Point)p1);
                        }
                    }
                }
            }
        }


        public IColorPalette Palette
        {
            get => GetValue(PaletteProperty) as IColorPalette;
            set
            {
                SetValue(PaletteProperty, value);
                InvalidateVisual();
            }
        }

        public double LineThickness
        {
            get => (double)GetValue(LineThicknessProperty);
            set => SetValue(LineThicknessProperty, value);
        }
        public double PointSize
        {
            get => (double)GetValue(PointSizeProperty);
            set => SetValue(PointSizeProperty, value);
        }
        public float CurveFittingTolerance
        {
            get => (float)GetValue(CurveFittingToleranceProperty);
            set => SetValue(CurveFittingToleranceProperty, value);
        }
        public ColorChannel Channel
        {
            get => (ColorChannel)GetValue(ChannelProperty);
            set => SetValue(ChannelProperty, value);
        }

        private Canvas Canvas => Template.FindName("Canvas", this) as Canvas;

        private Popup Tracker => Template.FindName("Tracker", this) as Popup;

        private IPointSeries _controlPoints = null;
        private Transform _transform = null;
        private Transform _inverse = null;
        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteChannelEditor editor)
            {
                if (e.OldValue is IColorPalette palette0)
                {
                    palette0.PropertyChanged -= editor.Palette_PropertyChanged;
                }

                if (e.NewValue is IColorPalette palette1)
                {
                    palette1.PropertyChanged += editor.Palette_PropertyChanged;
                }
                editor.InvalidateVisual();
            }
        }

        private void Palette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }


        private static bool IsLineThicknessValid(object value)
        {
            if (value is double v)
            {
                return v >= MinLineThickness;
            }

            return false;
        }


        private static bool IsPointSizeValid(object value)
        {
            if (value is double v)
            {
                return v >= MinPointSize;
            }

            return false;
        }
        private static bool IsToleranceValid(object value)
        {
            if (value is float v)
            {
                return v >= 0f;
            }

            return false;
        }
        protected override Size MeasureOverride(Size constraint)
        {
            double pw = Padding.Left + Padding.Right;
            double ph = Padding.Top + Padding.Bottom;
            Size size = constraint;
            if (Orientation == Orientation.Vertical)
            {
                if (double.IsInfinity(size.Width) || size.Width < _minWidth) size.Width = _minWidth + pw;
                if (double.IsInfinity(size.Height) || size.Height < _minHeight) size.Height = _minHeight + ph;
            }
            else
            {
                if (double.IsInfinity(size.Width) || size.Width < _minHeight) size.Width = _minHeight + pw;
                if (double.IsInfinity(size.Height) || size.Height < _minWidth) size.Height = _minWidth + ph;
            }

            return size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Thickness padding = Padding;
            Size size = new Size(arrangeBounds.Width - padding.Left - padding.Right, arrangeBounds.Height - padding.Top - padding.Bottom);
            DrawControl(size);
            return base.ArrangeOverride(arrangeBounds);
        }

        private void DrawControl(Size size)
        {
            Canvas canvas = Canvas;
            IColorPalette palette = Palette;
            if (canvas != null && palette != null)
            {
                canvas.Children.Clear();
                Range yRange = new Range(0.0, 1.0);
                Range xRange = new Range(0.0, palette.Size);
                _transform = TransformSupport.GetTransform(Orientation, xRange, yRange, size);
                _inverse = _transform.Inverse as Transform;
                _controlPoints = palette.GetControlPoints(Channel, CurveFittingTolerance);
                DrawGrid(size);
                DrawGraph(size, _controlPoints);
                if (_adorner == null)
                {
                    AdornerLayer myAdornerLayer = AdornerLayer.GetAdornerLayer(canvas);
                    _adorner = new ChannelEditorAdorner(canvas, Colors.Black, PointSize, LineThickness, _transform);
                    myAdornerLayer.Add(_adorner);
                }
            }
        }


        private void DrawGrid(Size size)
        {
            Canvas canvas = Canvas;
            IColorPalette palette = Palette;
            Rectangle r = new Rectangle()
            {
                Width = size.Width,
                Height = size.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 2.0,
                StrokeDashArray = { 1.0, 1.0 }
            };
            canvas.Children.Add(r);
        }

        private void DrawGraph(Size size, IPointSeries points)
        {
            Canvas canvas = Canvas;
            IColorPalette palette = Palette;
            if (canvas != null && palette != null && size.Width > 0.0 && size.Height > 0.0)
            {
                double lt = LineThickness;
                double ps = PointSize;
                ColorChannel cc = Channel;
                Point[] points1 = new Point[points.Count];
                for (int i = 0; i < points.Count; ++i)
                    points1[i] = _transform.Transform(points[i]);

                for (int p = 0; p < points.Count - 1; ++p)
                {
                    Line l = new Line()
                    {
                        X1 = points1[p].X,
                        Y1 = points1[p].Y,
                        X2 = points1[p + 1].X,
                        Y2 = points1[p + 1].Y,
                        Stroke = GetBrush(cc),
                        StrokeThickness = lt
                    };
                    canvas.Children.Add(l);
                }

                foreach (var p in points1)
                {
                    Ellipse e = new Ellipse() { Height = ps, Width = ps, Fill = GetBrush(Channel) };
                    Canvas.SetLeft(e, p.X - ps / 2.0);
                    Canvas.SetTop(e, p.Y - ps / 2.0);
                    canvas.Children.Add(e);
                }
            }
        }

        private Brush GetBrush(ColorChannel channel)
        {
            switch (channel)
            {
                case ColorChannel.Blue: return Brushes.Blue;
                case ColorChannel.Green: return Brushes.Green;
                case ColorChannel.Red: return Brushes.Red;
                default: return Brushes.Black;
            }
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public IUndoRedo UndoRedo
        {
            get => (IUndoRedo)GetValue(UndoRedoProperty);
            set => SetValue(UndoRedoProperty, value);
        }
    }
}
