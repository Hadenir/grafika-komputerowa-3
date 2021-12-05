using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GrafikaKomputerowa3
{
    public static class Histogram
    {
        private const int Width = 256;
        private const int Height = 200;

        public static unsafe WriteableBitmap[] CalculateHistograms(BitmapImage image)
        {
            var sourceImage = new WriteableBitmap(image);

            var reds = new int[256];
            var greens = new int[256];
            var blues = new int[256];

            var width = sourceImage.PixelWidth;
            var height = sourceImage.PixelHeight;
            var stride = sourceImage.Format.BitsPerPixel / 8;

            var buffer = (byte*)sourceImage.BackBuffer.ToPointer();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var r = buffer[(x + y * width) * stride + 0];
                    var g = buffer[(x + y * width) * stride + 1];
                    var b = buffer[(x + y * width) * stride + 2];

                    reds[r]++;
                    greens[g]++;
                    blues[b]++;
                }
            }

            var maxR = reds.Max();
            var maxG = greens.Max();
            var maxB = blues.Max();

            var redHistogram = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);
            var redBuffer = (uint*)redHistogram.BackBuffer.ToPointer();
            for (int x = 0; x < Width; x++)
            {
                var h = (int)((float)reds[x] / maxR * Height);
                for (int y = 0; y < Height; y++)
                {
                    redBuffer[x + (Height - y - 1) * Width] = y < h ? 0x00ff0000 : 0xffffffff;
                }
            }

            var greenHistogram = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);
            var greenBuffer = (uint*)greenHistogram.BackBuffer.ToPointer();
            for (int x = 0; x < Width; x++)
            {
                var h = (int)((float)greens[x] / maxG * Height);
                for (int y = 0; y < Height; y++)
                {
                    greenBuffer[x + (Height - y - 1) * Width] = y < h ? 0x0000ff00 : 0xffffffff;
                }
            }

            var blueHistogram = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgr32, null);
            var blueBuffer = (uint*)blueHistogram.BackBuffer.ToPointer();
            for (int x = 0; x < Width; x++)
            {
                var h = (int)((float)blues[x] / maxB * Height);
                for (int y = 0; y < Height; y++)
                {
                    blueBuffer[x + (Height - y - 1) * Width] = y < h ? 0x000000ff : 0xffffffff;
                }
            }

            try
            {
                redHistogram.Lock();
                redHistogram.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                greenHistogram.Lock();
                greenHistogram.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                blueHistogram.Lock();
                blueHistogram.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
            }
            finally
            {
                redHistogram.Unlock();
                greenHistogram.Unlock();
                blueHistogram.Unlock();
            }

            return new[] { redHistogram, greenHistogram, blueHistogram };
        }
    }
}