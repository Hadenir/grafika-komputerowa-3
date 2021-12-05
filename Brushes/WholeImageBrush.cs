using GrafikaKomputerowa3.Filters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GrafikaKomputerowa3.Brushes
{
    public class WholeImageBrush : IBrush
    {
        public Shape Tool { get; } = new Rectangle
        {
            Stroke = System.Windows.Media.Brushes.DarkGray,
            StrokeThickness = 2,
            StrokeDashArray = new DoubleCollection(new[] { 1.0 }),
            Visibility = Visibility.Hidden,
        };

        private readonly int width;
        private readonly int height;

        public WholeImageBrush(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void OnMouseMove(Point mousePos)
        {
            Tool.Width = width;
            Tool.Height = height;
            Canvas.SetLeft(Tool, 0);
            Canvas.SetTop(Tool, 0);
        }

        public void OnMouseDown(Point mousePos, IFilter filter)
        {
            try
            {
                filter.Lock();

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        filter.ApplyFilter(x, y);
                    }
                }
            }
            finally
            {
                filter.Unlock(0, 0, width, height);
            }
        }
    }
}