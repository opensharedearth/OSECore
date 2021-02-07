using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace OSEUIDesktop.WPF.Sample
{
    class SelectablePanel : Panel
    {
        public SelectablePanel()
            : base()
        {
            Focusable = true;
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Focus();
            e.Handled = true;
        }
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("in OnGotFocus");
            InvalidateVisual();
            base.OnGotFocus(e);
        }
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("in OnLostFocus");
            InvalidateVisual();
            base.OnLostFocus(e);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            Size panelDesiredSize = new Size();
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(availableSize);
                panelDesiredSize = child.DesiredSize;
            }

            return panelDesiredSize;
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                child.Arrange(new Rect(new Point(0, 0), child.DesiredSize));
            }
            return finalSize;
        }
        protected override void OnRender(DrawingContext dc)
        {
            System.Diagnostics.Debug.WriteLine("In OnRender, IsFocused = " + IsFocused.ToString());
            base.OnRender(dc);
            if (IsFocused)
            {
                var rect = LayoutInformation.GetLayoutSlot(this);
                var r = new Rect(1, 1 + Margin.Top, rect.Width - 2 - Margin.Left - Margin.Right, rect.Height - 2 - Margin.Top - Margin.Bottom);
                SolidColorBrush b = new SolidColorBrush(Colors.Transparent);
                Pen p = new Pen(new SolidColorBrush(Colors.Black), 1);
                p.DashStyle = new DashStyle(new double[] { 2, 3 }, 0);
                dc.DrawRectangle(b, p, r);
            }
        }
    }
}
