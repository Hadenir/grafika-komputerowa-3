using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GrafikaKomputerowa3.Filters
{
    public class MatrixFilter : IFilter
    {
        public float[] Matrix { get; } = new float[9] { 0, 0, 0, 0, 1, 0, 0, 0, 0 };
        public float Offset { get; set; } = 0;
        public float? Factor { get; set; }
        public WriteableBitmap? SourceBitmap { get; set; }
        public WriteableBitmap? TargetBitmap { get; set; }

        public void ApplyFilter(int x, int y)
        {
            if (SourceBitmap is null || TargetBitmap is null) return;

            var width = SourceBitmap.PixelWidth;
            var height = SourceBitmap.PixelHeight;

            if (x < 0 || y < 0 || x >= width || y >= height) return;

            var stride = SourceBitmap.Format.BitsPerPixel / 8;
            var idx = (x + y * width) * stride;
            var factor = Factor ?? Matrix.Sum();

            unsafe
            {
                var sourceBuffer = (byte*)SourceBitmap.BackBuffer.ToPointer();
                var targetBuffer = (byte*)TargetBitmap.BackBuffer.ToPointer();

                for (var i = 0; i < 3; i++)
                {
                    var sum = 0f;
                    if (y > 0 && x > 0) sum += sourceBuffer[idx - (width + 1) * stride + i] * Matrix[0];
                    if (y > 0) sum += sourceBuffer[idx - width * stride + i] * Matrix[1];
                    if (y > 0 && x < width - 1) sum += sourceBuffer[idx - (width - 1) * stride + i] * Matrix[2];
                    if (x > 0) sum += sourceBuffer[idx - stride + i] * Matrix[3];
                    sum += sourceBuffer[idx + i] * Matrix[4];
                    if (x < width - 1) sum += sourceBuffer[idx + stride + i] * Matrix[5];
                    if (y < height - 1 && x > 0) sum += sourceBuffer[idx + (width - 1) * stride + i] * Matrix[6];
                    if (y < height - 1) sum += sourceBuffer[idx + width * stride + i] * Matrix[7];
                    if (y < height - 1 && x < width - 1) sum += sourceBuffer[idx + (width + 1) * stride + i] * Matrix[8];

                    targetBuffer[idx + i] = (byte)Math.Clamp(Offset * 255 + sum / factor, 0, 255);
                }
            }
        }

        public void Lock()
        {
            TargetBitmap?.Lock();
        }

        public void Unlock(int x, int y, int w, int h)
        {
            if (SourceBitmap is null || TargetBitmap is null) return;

            var width = SourceBitmap.PixelWidth;
            var height = SourceBitmap.PixelHeight;

            TargetBitmap.AddDirtyRect(new Int32Rect(
                Math.Clamp(x, 0, width),
                Math.Clamp(y, 0, height),
                Math.Clamp(w, 0, width - Math.Clamp(x, 0, width)),
                Math.Clamp(h, 0, height - Math.Clamp(y, 0, height))
            ));
            TargetBitmap.Unlock();
        }
    }
}