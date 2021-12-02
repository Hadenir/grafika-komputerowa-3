using GrafikaKomputerowa3.Brushes;
using GrafikaKomputerowa3.Filters;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GrafikaKomputerowa3
{
    public partial class MainWindow : Window
    {
        public IBrush CurrentBrush { get; private set; }
        public IFilter CurrentFilter { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            CurrentBrush = new CircularBrush { Radius = 32 };
            ImageCanvas.Children.Add(CurrentBrush.Tool);
            CurrentFilter = null;
        }

        private Point GetCanvasMousePosition() => Mouse.GetPosition(ImageCanvas);

        // Event handlers:

        private void ImageCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            CurrentBrush.Tool.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void ImageCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            CurrentBrush.Tool.Visibility = Visibility.Hidden;
            e.Handled = true;
        }

        private void ImageCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentBrush.OnMouseDown(GetCanvasMousePosition(), CurrentFilter);
            e.Handled = true;
        }

        private void ImageCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CurrentBrush.OnMouseUp(GetCanvasMousePosition(), CurrentFilter);
            e.Handled = true;
        }

        private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            CurrentBrush.OnMouseMove(GetCanvasMousePosition());
            e.Handled = true;
        }

        private void CircularBrushRadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CurrentBrush is CircularBrush circular) circular.Radius = (int)e.NewValue;
            e.Handled = true;
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}