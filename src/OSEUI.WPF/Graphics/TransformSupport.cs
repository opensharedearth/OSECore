using OSECoreUI.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Order = OSECoreUI.Graphics.Order;
using Range = OSECoreUI.Graphics.Range;

namespace OSEUI.WPF.Graphics
{
    public class TransformSupport
    {
        public static Transform GetTransform(Orientation orientation, Range xRange, Range yRange, Size size)
        {
            if (orientation == Orientation.Horizontal)
            {
                if (xRange.Order == Order.Ascending && yRange.Order == Order.Ascending)
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(size.Width / xRange.Extent, -size.Height / yRange.Extent),
                            new TranslateTransform(0.0, size.Height)
                        }
                    };

                }
                else if(xRange.Order == Order.Ascending && yRange.Order == Order.Descending)
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(size.Width / xRange.Extent, size.Height / yRange.Extent),
                            new TranslateTransform(0.0, 0.0)                        }
                    };

                }
                else if (xRange.Order == Order.Descending && yRange.Order == Order.Ascending)
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(-size.Width / xRange.Extent, -size.Height / yRange.Extent),
                            new TranslateTransform(size.Width, size.Height)
                        }
                    };

                }
                else
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(-size.Width / xRange.Extent, size.Height / yRange.Extent),
                            new TranslateTransform(size.Width, 0.0)
                        }
                    };

                }
            }
            else
            {
                if (xRange.Order == Order.Ascending && yRange.Order == Order.Ascending)
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(size.Height / xRange.Extent, size.Width / yRange.Extent),
                            new RotateTransform(-90), new TranslateTransform(0.0, size.Height)
                        }
                    };

                }
                else if (xRange.Order == Order.Ascending && yRange.Order == Order.Descending)
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(size.Height / xRange.Extent, -size.Width / yRange.Extent), 
                            new RotateTransform(-90), new TranslateTransform(size.Width, size.Height)
                        }
                    };

                }
                else if (xRange.Order == Order.Descending && yRange.Order == Order.Ascending)
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(size.Height / xRange.Extent, -size.Width / yRange.Extent),
                            new RotateTransform(90), new TranslateTransform(0.0, 0.0)
                        }
                    };

                }
                else
                {
                    return new TransformGroup()
                    {
                        Children =
                        {
                            new TranslateTransform(-xRange.Min, -yRange.Min), new ScaleTransform(size.Height / xRange.Extent, size.Width / yRange.Extent),
                            new RotateTransform(90), new TranslateTransform(size.Width, 0.0)
                        }
                    };

                }
            }
        }
    }
}
