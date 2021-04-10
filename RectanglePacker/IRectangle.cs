using System.Drawing;

namespace RectanglePacker
{
    public interface IRectangle
    {
        Rectangle Bounds { get; }
        int MappedX { get; set; }
        int MappedY { get; set; }
        int Area { get; }
        int Perimiter { get; }
    }
}