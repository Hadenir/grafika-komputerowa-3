using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GrafikaKomputerowa3.Brushes
{
    public class CircularBrush : IBrush
    {
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
        }
    }
}