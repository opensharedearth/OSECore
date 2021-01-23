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
using OSECoreUI.Graphics;
using OSEUI.WPF.Colors;
using OSEUI.WPF.Graphics;
using Range = OSECoreUI.Graphics.Range;

namespace OSEUIControls.WPF
{
    public class ColorPaletteView : Control, INotifyPropertyChanged
    {
        private int _minWidth = 50;
        private int _minHeight = 100;
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register("Palette", typeof(IColorPalette), typeof(ColorPaletteView),
            new PropertyMetadata(null, new PropertyChangedCallback(OnPaletteChanged)));

        private Transform _transform;
        private Transform _inverse;

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteView view)
            {
                if (e.OldValue is IColorPalette palette0)
                {
                    palette0.PropertyChanged -= view.Palette_PropertyChanged;
                }

                if (e.NewValue is IColorPalette palette1)
                {
                    palette1.PropertyChanged += view.Palette_PropertyChanged;
                }
                view.InvalidateVisual();
            }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ColorPaletteView));
        static ColorPaletteView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPaletteView), new FrameworkPropertyMetadata(typeof(ColorPaletteView)));
        }

        public ColorPaletteView()
        {
            PreviewMouseLeftButtonDown += ColorPaletteView_PreviewMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += ColorPaletteView_PreviewMouseLeftButtonUp;
            PreviewMouseMove += ColorPaletteView_PreviewMouseMove;

        }

        private Popup Tracker => Template.FindName("Tracker", this) as Popup;
        private void ColorPaletteView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point p1 = _inverse.Transform(e.GetPosition(Image));
                Index = Math.Max(0, Math.Min(Palette.Size - 1,(int)p1.X));
                Color c = Palette[Index];
                Green = c.ScG;
                Red = c.ScR;
                Blue = c.ScB;
                OnPropertyChanged(nameof(Index));
                OnPropertyChanged(nameof(Red));
                OnPropertyChanged(nameof(Blue));
                OnPropertyChanged(nameof(Green));
                Point p = e.GetPosition(Image);
                Popup popup = Tracker;
                popup.HorizontalOffset = p.X + 30;
                popup.VerticalOffset = p.Y + 30;
            }
        }

        private void ColorPaletteView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Tracker.IsOpen = false;
                ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        private void ColorPaletteView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(Image);
            Point p1 = _inverse.Transform(p);
            Index = (int) p1.X;
            Color c = Palette.Colors[Index];
            Green = c.ScG;
            Red = c.ScR;
            Blue = c.ScB;
            Popup popup = Tracker;
            popup.PlacementTarget = this;
            popup.Placement = PlacementMode.RelativePoint;
            popup.HorizontalOffset = p.X + 30;
            popup.VerticalOffset = p.Y + 30;
            popup.DataContext = this;
            popup.IsOpen = true;
            CaptureMouse();
            e.Handled = true;
        }

        public int Index { get; private set; }
        public double Red { get; private set; }
        public double Green { get; private set; }
        public double Blue { get; private set; }

        public IColorPalette Palette
        {
            get => GetValue(PaletteProperty) as IColorPalette;
            set
            {
                SetValue(PaletteProperty, value);
                InvalidateVisual();
            }
        }

        private Image Image => Template.FindName("Image",this) as Image;

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
            Arrange(arrangeBounds);
            return base.ArrangeOverride(arrangeBounds);
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set
            {
                if (Orientation != value)
                {
                    SetValue(OrientationProperty, value);
                }
            }
        }
        private void Palette_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            InvalidateVisual();
        }

        private void Arrange(Size size)
        {
            double pw = Padding.Left + Padding.Right;
            double ph = Padding.Top + Padding.Bottom;
            Image image = Image;
            IColorPalette palette = Palette;
            double width = size.Width - pw;
            double height = size.Height - ph;
            Orientation orientation = Orientation;
            if (image != null && palette != null && width > 0.0 && height > 0.0)
            {
                Color[] colors = palette.Colors;
                int length = colors.Length;
                Range yRange = new Range(0.0, 1.0);
                Range xRange = new Range(0.0, length);
                Transform t = TransformSupport.GetTransform(Orientation, xRange, yRange, new Size(width, height));
                _transform = t;
                _inverse = t.Inverse as Transform;
                ;
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                Rect r0 = new Rect(t.Transform(new  Point(0.0, 0.0)), t.Transform(new Point(1.0, 1.0)));
                Rect r1 = new Rect(t.Transform(new Point(1.0, 0.0)), t.Transform(new Point(2.0, 1.0)));
                Vector v = new Vector(r1.Left - r0.Left, r1.Top - r0.Top);
                Rect r = r0;
                for (int i = 0; i < length; ++i)
                {
                    Brush b = new SolidColorBrush(colors[i]);
                    Pen p = new Pen(b, 1.0);
                    drawingContext.DrawRectangle(b, p, r);
                    r = Rect.Offset(r, v);
                }
                drawingContext.Close();
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(drawingVisual);
                image.Source = bmp;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
