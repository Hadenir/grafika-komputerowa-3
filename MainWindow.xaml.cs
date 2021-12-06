using GrafikaKomputerowa3.Brushes;
using GrafikaKomputerowa3.Filters;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            CurrentBrush = new CircularBrush { Radius = 32 };
            CurrentFilter = new MatrixFilter();

            InitializeComponent();

            ImageGrid.Children.Add(CurrentBrush.Tool);
        }

        private Point GetCanvasMousePosition() => Mouse.GetPosition(Image);

        // Event handlers:

        private void ImageCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            //CurrentBrush.Tool.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void ImageCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            //CurrentBrush.Tool.Visibility = Visibility.Hidden;
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
                CurrentFilter.Matrix[0] = 1;
                CurrentFilter.Matrix[1] = 2;
                CurrentFilter.Matrix[2] = 1;
                CurrentFilter.Matrix[3] = 2;
                CurrentFilter.Matrix[4] = 4;
                CurrentFilter.Matrix[5] = 2;
                CurrentFilter.Matrix[6] = 1;
                CurrentFilter.Matrix[7] = 2;
                CurrentFilter.Matrix[8] = 1;
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
            else if (sender == CustomFilterRadio)
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
                if (Image.Source is null)
                {
                    WholeImageBrushRadio.IsChecked = false;
                    CircularBrushRadio.IsChecked = true;
                    return;
                }

                ImageGrid.Children.Remove(CurrentBrush.Tool);
                CurrentBrush = new WholeImageBrush((int)Image.Source.Width, (int)Image.Source.Height);
                ImageGrid.Children.Add(CurrentBrush.Tool);
            }
            else if (sender == CircularBrushRadio)
            {
                ImageGrid.Children.Remove(CurrentBrush.Tool);
                CurrentBrush = new CircularBrush { Radius = (int)CircularBrushRadiusSlider.Value };
                ImageGrid.Children.Add(CurrentBrush.Tool);
            }

            e.Handled = true;
        }

        private void FilterOffsetBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CurrentFilter.Offset = Convert.ToSingle(e.NewValue);
            e.Handled = true;
        }

        private void FilterFactorBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (FilterAutoFactorCheckBox.IsChecked != false) return;

            CurrentFilter.Factor = Convert.ToSingle(e.NewValue);
            e.Handled = true;
        }

        private void FilterAutoFactorCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (FilterAutoFactorCheckBox.IsChecked != true) CurrentFilter.Factor = Convert.ToSingle(FilterFactorBox?.Value);
            else CurrentFilter.Factor = null;
            e.Handled = true;
        }

        private unsafe void ReduceButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentFilter.SourceBitmap is null) return;

            var k = (int)KSlider.Value;

            var bitmap = CurrentFilter.SourceBitmap;
            var buffer = (int*)bitmap.BackBuffer.ToPointer();
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;

            var ka = k / 8;
            var kb = k % 8;

            // Calculating popularity of colors.
            var counts = new Dictionary<int, int>();
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var color = buffer[x + y * width];
                    if (counts.ContainsKey(color))
                        counts[color]++;
                    else
                        counts.Add(color, 1);
                }
            }

            // Choosing mostp popular colors from all 8 quadrants.
            var colors1 = counts.Where(x => IsInNthQuadrant(x.Key, 0))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors2 = counts.Where(x => IsInNthQuadrant(x.Key, 1))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors3 = counts.Where(x => IsInNthQuadrant(x.Key, 2))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors4 = counts.Where(x => IsInNthQuadrant(x.Key, 3))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors5 = counts.Where(x => IsInNthQuadrant(x.Key, 4))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors6 = counts.Where(x => IsInNthQuadrant(x.Key, 5))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors7 = counts.Where(x => IsInNthQuadrant(x.Key, 6))
                            .OrderByDescending(x => x.Value)
                            .Take(ka)
                            .Select(x => x.Key);
            var colors8 = counts.Where(x => IsInNthQuadrant(x.Key, 7))
                            .OrderByDescending(x => x.Value)
                            .Take(ka + kb)
                            .Select(x => x.Key);

            // Create the palette of colors.
            var palette = colors1
                            .Concat(colors2)
                            .Concat(colors3)
                            .Concat(colors4)
                            .Concat(colors5)
                            .Concat(colors6)
                            .Concat(colors7)
                            .Concat(colors8)
                            .ToList();

            // Mapping original colors, to the closest ones from the palette.
            try
            {
                bitmap.Lock();

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var color = buffer[x + y * width];
                        var col = palette[0];
                        var md = Dist(color, col);

                        foreach (var p in palette)
                        {
                            var d = Dist(color, p);
                            if (d < md)
                            {
                                col = p;
                                md = d;
                            }
                        }

                        buffer[x + y * width] = col;
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }

            CurrentFilter.TargetBitmap = bitmap.Clone();
            Image.Source = CurrentFilter.TargetBitmap;

            var histograms = Histogram.CalculateHistograms(ConvertWriteableBitmapToBitmapImage(bitmap));
            HistogramRedImage.Source = histograms[0];
            HistogramGreenImage.Source = histograms[1];
            HistogramBlueImage.Source = histograms[2];

            e.Handled = true;
        }

        private bool IsInNthQuadrant(int color, int q)
        {
            var r = (color >> 16) & 0xff;
            var g = (color >> 8) & 0xff;
            var b = (color >> 0) & 0xff;

            switch (q)
            {
                case 0:
                    return r < 0x80 && g < 0x80 && b < 0x80;

                case 1:
                    return r >= 0x80 && g < 0x80 && b < 0x80;

                case 2:
                    return r < 0x80 && g >= 0x80 && b < 0x80;

                case 3:
                    return r >= 0x80 && g >= 0x80 && b < 0x80;

                case 4:
                    return r < 0x80 && g < 0x80 && b >= 0x80;

                case 5:
                    return r >= 0x80 && g < 0x80 && b >= 0x80;

                case 6:
                    return r < 0x80 && g >= 0x80 && b >= 0x80;

                case 7:
                    return r >= 0x80 && g >= 0x80 && b >= 0x80;

                default:
                    throw new Exception("Shouldn't happen!");
            }
        }

        private int Dist(int color1, int color2)
        {
            var r1 = (color1 >> 16) & 0xff;
            var g1 = (color1 >> 8) & 0xff;
            var b1 = (color1 >> 0) & 0xff;

            var r2 = (color2 >> 16) & 0xff;
            var g2 = (color2 >> 8) & 0xff;
            var b2 = (color2 >> 0) & 0xff;

            return (r1 - r2) * (r1 - r2) + (g1 - g2) * (g1 - g2) + (b1 - b2) * (b1 - b2);
        }

        public BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap wbm)
        {
            BitmapImage bmImage = new BitmapImage();
            using (MemoryStream stream = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(wbm));
                encoder.Save(stream);
                bmImage.BeginInit();
                bmImage.CacheOption = BitmapCacheOption.OnLoad;
                bmImage.StreamSource = stream;
                bmImage.EndInit();
                bmImage.Freeze();
            }
            return bmImage;
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