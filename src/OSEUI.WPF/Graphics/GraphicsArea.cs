using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OSEUI.WPF.Graphics
{
    public class GraphicsArea
    {
        private Rect _boundary;
        private List<Rect> _allocations = new List<Rect>();
        private Rect _free;

        public GraphicsArea(Rect boundary)
        {
            _boundary = boundary;
            _free = _boundary;
        }

        public GraphicsArea(Size size)
        {
            _boundary = new Rect(size);
            _free = _boundary;
        }

        public Rect Boundary => _boundary;

        public Rect Free => _free;

        public bool Allocate(Rect r)
        {
            if (Rect.Union(_free, r) == _free)
            {
                _allocations.Add(r);
                UpdateFree(r);
                return true;
            }
            else if (Rect.Union(_boundary, r) == _boundary)
            {
                foreach (Rect r1 in _allocations)
                {
                    Rect r2 = Rect.Intersect(r, r1);
                    if(!r2.IsEmpty && r2.Width * r2.Height > 0.0)
                        return false;
                }

                _allocations.Add(r);
                UpdateFree(r);
                return true;
            }

            return false;
        }

        private void UpdateFree(Rect r)
        {
            double left = r.Left - _free.Left;
            double right = _free.Right - r.Right;
            double top = r.Top - _free.Top;
            double bottom = _free.Bottom - r.Bottom;
            double width = Math.Max(left, right);
            double height = Math.Max(top, bottom);
            if (width > height)
            {
                if (left > right)
                    _free = new Rect(_free.Left, _free.Top, left, _free.Height);
                else
                    _free = new Rect(r.Right, _free.Top, right, _free.Height);
            }
            else
            {
                if (top > bottom)
                    _free = new Rect(_free.Left, _free.Top, _free.Width, top);
                else
                    _free = new Rect(_free.Left, r.Bottom, _free.Width, bottom);

            }
        }
    }
}
