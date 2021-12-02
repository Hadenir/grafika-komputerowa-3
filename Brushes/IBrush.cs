using GrafikaKomputerowa3.Filters;
using System.Windows;
using System.Windows.Shapes;

namespace GrafikaKomputerowa3.Brushes
{
    public interface IBrush
    {
        Shape Tool { get; }

        void OnMouseDown(Point mousePos, IFilter filter) { }

        void OnMouseMove(Point mousePos) { }

        void OnMouseUp(Point mousePos, IFilter filter) { }
    }
}