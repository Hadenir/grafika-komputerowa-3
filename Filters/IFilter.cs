using System.Windows;

namespace GrafikaKomputerowa3.Filters
{
    public interface IFilter
    {
        void ApplyFilter(Point point);

        byte[] GetFilteredBuffer();

        void SwapBuffer();
    }

    public abstract class FilterBase : IFilter
    {
        protected byte[] sourceBuffer;
        protected byte[] targetBuffer;

        protected FilterBase(byte[] buffer)
        {
            sourceBuffer = buffer;
            targetBuffer = new byte[buffer.Length];
            sourceBuffer.CopyTo(targetBuffer, 0);
        }

        public abstract void ApplyFilter(Point point);

        public byte[] GetFilteredBuffer() => targetBuffer;

        public void SwapBuffer()
        {
            (targetBuffer, sourceBuffer) = (sourceBuffer, targetBuffer);
            sourceBuffer.CopyTo(targetBuffer, 0);
        }
    }
}