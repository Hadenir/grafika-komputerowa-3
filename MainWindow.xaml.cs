using GrafikaKomputerowa3.Brushes;
using GrafikaKomputerowa3.Filters;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace GrafikaKomputerowa3
{
    public partial class MainWindow : Window
    {
        public IBrush CurrentBrush { get; private set; }
        public IFilter CurrentFilter { get; private set; }

        private WriteableBitmap? sourceBitmap;
        private WriteableBitmap? writeableBitmap;

        public MainWindow()
        {
            InitializeComponent();

            CurrentBrush = new CircularBrush { Radius = 32 };
            ImageCanvas.Children.Add(CurrentBrush.Tool);
            CurrentFilter = new MatrixFilter();
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

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Image |*.jpg;*.bmp;*.png",
                InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            };
            if (fileDialog.ShowDialog() != true) return;

            var image = new BitmapImage(new Uri(fileDialog.FileName));
            sourceBitmap = new WriteableBitmap(image);
            writeableBitmap = sourceBitmap.Clone();
            Image.Source = writeableBitmap;

            CurrentFilter.SourceBitmap = sourceBitmap;
            CurrentFilter.TargetBitmap = writeableBitmap;

            CurrentBrush = new CircularBrush { Radius = (int)CircularBrushRadiusSlider.Value };

            var histograms = Histogram.CalculateHistograms(image);
            HistogramRedImage.Source = histograms[0];
            HistogramGreenImage.Source = histograms[1];
            HistogramBlueImage.Source = histograms[2];

            e.Handled = true;
        }

        private void FilterRadio_Click(object sender, RoutedEventArgs e)
        {
            if (sender == IdentityFilterRadio)
            {
                CurrentFilter.Matrix[0] = 0;
                CurrentFilter.Matrix[1] = 0;
                CurrentFilter.Matrix[2] = 0;
                CurrentFilter.Matrix[3] = 0;
                CurrentFilter.Matrix[4] = 1;
                CurrentFilter.Matrix[5] = 0;
                CurrentFilter.Matrix[6] = 0;
                CurrentFilter.Matrix[7] = 0;
                CurrentFilter.Matrix[8] = 0;
            }
            else if (sender == BlurFilterRadio)
            {
                CurrentFilter.Matrix[0] = 0;
                CurrentFilter.Matrix[1] = 1;
                CurrentFilter.Matrix[2] = 0;
                CurrentFilter.Matrix[3] = 1;
                CurrentFilter.Matrix[4] = 4;
                CurrentFilter.Matrix[5] = 1;
                CurrentFilter.Matrix[6] = 0;
                CurrentFilter.Matrix[7] = 1;
                CurrentFilter.Matrix[8] = 0;
            }
            else if (sender == SharpenFilterRadio)
            {
                CurrentFilter.Matrix[0] = 0;
                CurrentFilter.Matrix[1] = -1;
                CurrentFilter.Matrix[2] = 0;
                CurrentFilter.Matrix[3] = -1;
                CurrentFilter.Matrix[4] = 5;
                CurrentFilter.Matrix[5] = -1;
                CurrentFilter.Matrix[6] = 0;
                CurrentFilter.Matrix[7] = -1;
                CurrentFilter.Matrix[8] = 0;
            }
            else if (sender == ReliefFilterRadio)
            {
                CurrentFilter.Matrix[0] = -1;
                CurrentFilter.Matrix[1] = -1;
                CurrentFilter.Matrix[2] = 0;
                CurrentFilter.Matrix[3] = -1;
                CurrentFilter.Matrix[4] = 1;
                CurrentFilter.Matrix[5] = 1;
                CurrentFilter.Matrix[6] = 0;
                CurrentFilter.Matrix[7] = 1;
                CurrentFilter.Matrix[8] = 1;
            }
            else if (sender == EdgeDetectionFilterRadio)
            {
                CurrentFilter.Matrix[0] = -1;
                CurrentFilter.Matrix[1] = -1;
                CurrentFilter.Matrix[2] = -1;
                CurrentFilter.Matrix[3] = -1;
                CurrentFilter.Matrix[4] = 8;
                CurrentFilter.Matrix[5] = -1;
                CurrentFilter.Matrix[6] = -1;
                CurrentFilter.Matrix[7] = -1;
                CurrentFilter.Matrix[8] = -1;
            }
            else if (sender == EdgeDetectionFilterRadio)
            {
                CurrentFilter.Matrix[0] = (float)CustomFilterABox.Value!.Value;
                CurrentFilter.Matrix[1] = (float)CustomFilterBBox.Value!.Value;
                CurrentFilter.Matrix[2] = (float)CustomFilterCBox.Value!.Value;
                CurrentFilter.Matrix[3] = (float)CustomFilterDBox.Value!.Value;
                CurrentFilter.Matrix[4] = (float)CustomFilterEBox.Value!.Value;
                CurrentFilter.Matrix[5] = (float)CustomFilterFBox.Value!.Value;
                CurrentFilter.Matrix[6] = (float)CustomFilterGBox.Value!.Value;
                CurrentFilter.Matrix[7] = (float)CustomFilterHBox.Value!.Value;
                CurrentFilter.Matrix[8] = (float)CustomFilterIBox.Value!.Value;
            }

            e.Handled = true;
        }

        private void CustomFilter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (CustomFilterRadio.IsChecked != true) return;

            CurrentFilter.Matrix[0] = (float)CustomFilterABox.Value!.Value;
            CurrentFilter.Matrix[1] = (float)CustomFilterBBox.Value!.Value;
            CurrentFilter.Matrix[2] = (float)CustomFilterCBox.Value!.Value;
            CurrentFilter.Matrix[3] = (float)CustomFilterDBox.Value!.Value;
            CurrentFilter.Matrix[4] = (float)CustomFilterEBox.Value!.Value;
            CurrentFilter.Matrix[5] = (float)CustomFilterFBox.Value!.Value;
            CurrentFilter.Matrix[6] = (float)CustomFilterGBox.Value!.Value;
            CurrentFilter.Matrix[7] = (float)CustomFilterHBox.Value!.Value;
            CurrentFilter.Matrix[8] = (float)CustomFilterIBox.Value!.Value;
        }

        private void BrushRadio_Click(object sender, RoutedEventArgs e)
        {
            if (sender == WholeImageBrushRadio)
            {
                CurrentBrush = new WholeImageBrush((int)Image.Source.Width, (int)Image.Source.Height);
            }
            else if (sender == CircularBrushRadio)
            {
                CurrentBrush = new CircularBrush { Radius = (int)CircularBrushRadiusSlider.Value };
            }

            e.Handled = true;
        }

        private void FilterOffsetBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CurrentFilter.Offset = (float)e.NewValue;
            e.Handled = true;
        }

        private void FilterFactorBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CurrentFilter.Factor = (float)e.NewValue;
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