using GrafikaKomputerowa3.Filters;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GrafikaKomputerowa3.Brushes
{
    public class CircularBrush : IBrush
    {
        private bool mouseDown = false;
        private IFilter? filter = null;

        public int Radius;

        public Shape Tool { get; } = new Ellipse
        {
            Stroke = System.Windows.Media.Brushes.DarkGray,
            StrokeThickness = 2,
            StrokeDashArray = new DoubleCollection(new[] { 1.0 }),
            Visibility = Visibility.Hidden,
        };

        public void OnMouseMove(Point mousePos)
        {
            Tool.Width = 2 * Radius;
            Tool.Height = 2 * Radius;
            Canvas.SetLeft(Tool, mousePos.X - Radius);
            Canvas.SetTop(Tool, mousePos.Y - Radius);

            if (!mouseDown) return;

            var centerX = (int)mousePos.X;
            var centerY = (int)mousePos.Y;
            var startX = (int)(mousePos.X - Radius);
            var startY = (int)(mousePos.Y - Radius);

            ApplyFilter(centerX, centerY, startX, startY);
        }

        public void OnMouseDown(Point mousePos, IFilter filter)
        {
            var centerX = (int)mousePos.X;
            var centerY = (int)mousePos.Y;
            var startX = (int)(mousePos.X - Radius);
            var startY = (int)(mousePos.Y - Radius);

            mouseDown = true;
            this.filter = filter;

            ApplyFilter(centerX, centerY, startX, startY);
        }

        public void OnMouseUp(Point mousePos, IFilter filter)
        {
            mouseDown = false;
            this.filter = null;
        }

        private void ApplyFilter(int centerX, int centerY, int startX, int startY)
        {
            if (filter is null) return;

            try
            {
                filter.Lock();

                for (var y = startY; y < startY + 2 * Radius; y++)
                {
                    for (var x = startX; x < startX + 2 * Radius; x++)
                    {
                        if ((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY) <= Radius * Radius)
                        {
                            filter.ApplyFilter(x, y);
                        }
                    }
                }
            }
            finally
            {
                filter.Unlock(startX, startY, 2 * Radius, 2 * Radius);
            }
        }
    }
}