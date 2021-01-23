using System;
using System.Collections.Generic;
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
using OSEUI.WPF.Colors;

namespace OSEUIControls.WPF
{
    public class ColorPaletteEditor : Control
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ColorPaletteEditor));
        public static readonly DependencyProperty PaletteProperty = DependencyProperty.Register("Palette", typeof(IColorPalette), typeof(ColorPaletteEditor),
                new PropertyMetadata(null, new PropertyChangedCallback(OnPaletteChanged)));
        static ColorPaletteEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPaletteEditor), new FrameworkPropertyMetadata(typeof(ColorPaletteEditor)));
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
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

        private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPaletteEditor editor)
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
        public ColorPaletteChannelEditor RedChannelEditor => Template.FindName("RedChannel",this) as ColorPaletteChannelEditor;
        public ColorPaletteChannelEditor BlueChannelEditor => Template.FindName("BlueChannel", this) as ColorPaletteChannelEditor;
        public ColorPaletteChannelEditor GreenChannelEditor => Template.FindName("GreenChannel", this) as ColorPaletteChannelEditor;
    }
}
