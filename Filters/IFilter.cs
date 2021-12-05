using System.Windows;
using System.Windows.Media.Imaging;

namespace GrafikaKomputerowa3.Filters
{
    public interface IFilter
    {
        float[] Matrix { get; }
        float Offset { get; set; }
        float? Factor { get; set; }
        WriteableBitmap? SourceBitmap { get; set; }
        WriteableBitmap? TargetBitmap { get; set; }

        void Lock();

        void Unlock(int x, int y, int w, int h);

        void ApplyFilter(int x, int y);
    }
}