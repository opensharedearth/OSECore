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
    public class FocusAdorner : Adorner
    {
        public FocusAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            IsHitTestVisible = false;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            var element = this.AdornedElement as FrameworkElement;
            var rect = LayoutInformation.GetLayoutSlot(element);
            var margin = element.Margin;
            var r = new Rect(1, 1 + margin.Top, rect.Width - 2 - margin.Left - margin.Right, rect.Height - 2 - margin.Top - margin.Bottom);
            SolidColorBrush b = new SolidColorBrush(Colors.Transparent);
            Pen p = new Pen(new SolidColorBrush(Colors.Black), 1);
            p.DashStyle = new DashStyle(new double[] { 2, 3 }, 0);
            drawingContext.DrawRectangle(b, p, r);
        }
    }
}
