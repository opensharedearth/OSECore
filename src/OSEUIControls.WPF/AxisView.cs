using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OSEUI.WPF.Graphics;
using OSECoreUI.Graphics;
using Size = System.Windows.Size;
using Range = OSECoreUI.Graphics.Range;

namespace OSEUIControls.WPF
{
    public class AxisView : Control
    {
        private string _labelFormat = "G5";
        private int _labelDigits = 0;
        private double _minTickWidth = 0.0;
        private Size _labelSize = new Size(0.0, 0.0);
        private Size _minControlSize = new Size(0.0, 0.0);
        private Thickness _labelMargin = new Thickness(5.0, 5.0, 5.0, 5.0);
        private double _boundaryTickWidthFactor = 0.75;
        private double _tickWidthFactor = 0.25;
        private double _strokeThickness = 2.0;
        private Brush _stroke = Brushes.Black;
        private Thickness _labelPadding = new Thickness(2.0);
        public static readonly DependencyProperty AxisProperty = DependencyProperty.Register("Axis", typeof(Axis), typeof(AxisView),
            new PropertyMetadata(new Axis(), new PropertyChangedCallback(OnAxisChanged)));

        public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register("Edge", typeof(Edge),
            typeof(AxisView),
            new PropertyMetadata(Edge.Left));
        public static readonly DependencyProperty OrderProperty = DependencyProperty.Register("Order", typeof(Order), typeof(AxisView),
            new PropertyMetadata(Order.Ascending));


        static AxisView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AxisView), new FrameworkPropertyMetadata(typeof(AxisView)));
        }

        public AxisView()
        {
        }


        public Axis Axis
        {
            get => (Axis)GetValue(AxisProperty);
            set
            {
                SetValue(AxisProperty, value);
                InvalidateArrange();
            }
        }

        public Edge Edge
        {
            get => (Edge)GetValue(EdgeProperty);
            set => SetValue(EdgeProperty, value);
        }
        public Order Order
        {
            get => (Order)GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }

        public Orientation Orientation => Edge == Edge.Left || Edge == Edge.Right ? Orientation.Vertical : Orientation.Horizontal;

        private static void OnAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AxisView view)
            {
                if (e.OldValue is Axis a0)
                {
                    a0.PropertyChanged -= view.Axis_PropertyChanged;
                }

                if (e.NewValue is Axis a1)
                {
                    a1.PropertyChanged += view.Axis_PropertyChanged;
                }
                view.InvalidateArrange();
            }
        }

        private void Axis_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CalulateSizes();
        }

        private void CalulateSizes()
        {
            try
            {
                if (Axis is Axis a)
                {
                    _labelDigits = 0;
                    _labelSize = new Size(0.0, 0.0);
                    foreach (double v in a.GetTicks())
                    {
                        string s = v.ToString(_labelFormat);
                        Label l = new Label() {Content = s, Padding = _labelPadding};
                        l.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        _labelDigits = Math.Max(_labelDigits, s.Length);
                        _labelSize.Width = Math.Max(_labelSize.Width, l.DesiredSize.Width);
                        _labelSize.Height = Math.Max(_labelSize.Height, l.DesiredSize.Height);

                    }
                    _minTickWidth = _labelSize.Width / _labelDigits;
                    if (Orientation == Orientation.Vertical)
                    {
                        _minControlSize.Width = _labelSize.Width + _minTickWidth +
                                                _labelMargin.Left + _labelMargin.Right;
                        _minControlSize.Height =
                            (_labelSize.Height + _labelMargin.Top + _labelMargin.Bottom) * a.ActualTicks;
                    }
                    else
                    {
                        _minControlSize.Width = (_labelSize.Width +
                                                 _labelMargin.Left + _labelMargin.Right) * a.ActualTicks;
                        _minControlSize.Height =
                            _labelSize.Height + _labelMargin.Top + _labelMargin.Bottom + _minTickWidth;

                    }
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("Invalidate parameters for axis view", e);
            }
        }

        private Canvas Canvas => Template.FindName("Canvas", this) as Canvas;

        protected override Size MeasureOverride(Size constraint)
        {
            CalulateSizes();
            double pw = Padding.Left + Padding.Right;
            double ph = Padding.Top + Padding.Bottom;
            double minWidth = _minControlSize.Width + pw;
            double minHeight = _minControlSize.Height + ph;
            double width = double.IsInfinity(constraint.Width)
                ? minWidth
                : constraint.Width;
            double height = double.IsInfinity(constraint.Height)
                ? minHeight
                : constraint.Height;
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            double pw = Padding.Left + Padding.Right;
            double ph = Padding.Top + Padding.Bottom;
            double minWidth = _minControlSize.Width + pw;
            double minHeight = _minControlSize.Height + ph;
            Size size = Orientation == Orientation.Horizontal
                ? new Size(arrangeBounds.Width, minHeight)
                : new Size(minWidth, arrangeBounds.Height);
            Arrange(size);
            return base.ArrangeOverride(size);
        }

        private void Arrange(Size bounds)
        {
            double pw = Padding.Left + Padding.Right;
            double ph = Padding.Top + Padding.Bottom;
            Canvas canvas = Canvas;
            Axis a = Axis;
            double width = bounds.Width - pw;
            double height = bounds.Height - ph;
            if (canvas != null && a != null && width > 0.0 && height > 0.0)
            {
                canvas.Children.Clear();
                Range yRange = new Range(Order, a.Min, a.Max);
                Range xRange = new Range(Edge == Edge.Left || Edge == Edge.Bottom ? Order.Ascending : Order.Descending, 0.0, 1.0);
                Transform t = TransformSupport.GetTransform(Orientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal, xRange, yRange, new Size(width,height));
                DrawAxisLines(canvas, t, a, width, height);
                if (Orientation == Orientation.Vertical)
                    DrawVerticalAxisLabels(canvas, t, a, width, height);
                else
                    DrawHorizontalAxisLabels(canvas, t, a, width, height);
            }
        }

        private void DrawHorizontalAxisLabels(Canvas canvas, Transform t, Axis a, double width, double height)
        {
            GraphicsArea ga = new GraphicsArea(new Size(width, height));
            CreateHorizontalLabel(canvas, t, a.Min, width, height, Edge.Left, ga);
            CreateHorizontalLabel(canvas, t, a.Max, width, height, Edge.Right, ga);
            double[] ticks = a.GetTicks();
            for (int i = 1; i < ticks.Length; ++i)
            {
                CreateHorizontalLabel(canvas, t, ticks[i], width, height, Edge.Left, ga);
            }
        }
        private void DrawVerticalAxisLabels(Canvas canvas, Transform t, Axis a, double width, double height)
        {
            GraphicsArea ga = new GraphicsArea(new Size(width, height));
            CreateVerticalLabel(canvas, t, a.Min, width, height, ga);
            CreateVerticalLabel(canvas, t, a.Max, width, height, ga);
            double[] ticks = a.GetTicks();
            for (int i = 1; i < ticks.Length - 1; ++i)
            {
                CreateVerticalLabel(canvas, t, ticks[i], width, height, ga);
            }
        }


        private void CreateVerticalLabel(Canvas canvas, Transform t, double v, double width, double height, GraphicsArea ga)
        {
            Point p = t.Transform(new Point(0.0, v));
            bool isUpper = Math.Abs(p.Y) < _labelMargin.Top;
            bool isLower = Math.Abs(p.Y - height) < _labelMargin.Bottom;
            double left = Edge == Edge.Left ? 0.0 : width * _tickWidthFactor;
            double top = isUpper
                ? _strokeThickness + _labelMargin.Top
                : isLower
                    ? height - _labelSize.Height - _labelMargin.Bottom
                    : p.Y - _labelSize.Height / 2.0;
            Rect r = new Rect(new Point(left, top), _labelSize);
            if (ga.Allocate(r))
            {
                Label label = new Label()
                {
                    Content = v.ToString(_labelFormat),
                    Padding = _labelPadding,
                    HorizontalContentAlignment = HorizontalAlignment.Right,
                    Width = _labelSize.Width
                };
                Canvas.SetLeft(label, left);
                Canvas.SetTop(label, top);
                canvas.Children.Add(label);
            }
        }
        private void CreateHorizontalLabel(Canvas canvas, Transform t, double v, double width, double height, Edge edge, GraphicsArea ga)
        {
            Point p = t.Transform(new Point(0.0, v));
            bool isLeft = Math.Abs(p.X) < _labelMargin.Left;
            bool isRight = Math.Abs(p.X - width) < _labelMargin.Right;
            double top = Edge == Edge.Top ? 0.0 : height - _labelSize.Height - _labelMargin.Bottom;
            double left = isLeft
                ? _strokeThickness + _labelMargin.Left
                : isRight
                    ? width - _labelSize.Width - _labelMargin.Right
                    : p.X - _labelSize.Width / 2.0;
            double right = left + _labelSize.Width;
            Rect r = new Rect(new Point(left, top), _labelSize);
            if (ga.Allocate(r))
            {
                Label label = new Label()
                {
                    Content = v.ToString(_labelFormat),
                    Padding = _labelPadding,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Width = _labelSize.Width
                };
                Canvas.SetLeft(label, left);
                Canvas.SetTop(label, top);
                canvas.Children.Add(label);
            }
        }
        private void DrawAxisLines(Canvas canvas, Transform t, Axis a, double width, double height)
        {
            double min = a.Min;
            double max = a.Max;
            double inc = a.TickIncrement;
            double[] ticks = a.GetTicks();
            Line axisLine = CreateLine(t, 1.0, min, 1.0, max);
            canvas.Children.Add(axisLine);
            Line topTick = CreateLine(t, 1.0 - _boundaryTickWidthFactor, min, 1.0, min);
            canvas.Children.Add(topTick);
            Line bottomTick = CreateLine(t, 1.0 - _boundaryTickWidthFactor, max, 1.0, max);
            canvas.Children.Add(bottomTick);
            for (int i = 1; i < ticks.Length - 1; ++i)
            {
                Line tick = CreateLine(t, 1.0 - _tickWidthFactor, ticks[i],1.0, ticks[i]);
                canvas.Children.Add(tick);
            }
        }

        private Line CreateLine(Transform t, double x1, double y1, double x2, double y2)
        {
            Point p1 = t.Transform(new Point(x1, y1));
            Point p2 = t.Transform(new Point(x2, y2));
            return new Line()
            {
                X1 = p1.X,
                X2 = p2.X,
                Y1 = p1.Y,
                Y2 = p2.Y,
                StrokeThickness = _strokeThickness,
                Stroke = _stroke,
                StrokeEndLineCap = PenLineCap.Flat
            };
        }


    }
}
