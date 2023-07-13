using System.Drawing;

namespace RectanglePacker.Defaults;

public class DefaultRectangle : IRectangle
{
    private Rectangle _rect;
    private int _area, _perimiter;

    public Rectangle Bounds
    {
        get => _rect;
        set
        {
            _rect = value;
            _area = Bounds.Width * Bounds.Height;
            _perimiter = 2 * Bounds.Width + 2 * Bounds.Height;
        }
    }

    public Rectangle MappedBounds => new(MappedX, MappedY, Width, Height);

    public int Area => _area;
    public int Perimiter => _perimiter;
    public int Width => Bounds.Width;
    public int Height => Bounds.Height;
    public int MappedX { get; set; }
    public int MappedY { get; set; }

    public DefaultRectangle(int x, int y, int width, int height)
        : this(new Rectangle(x, y, width, height)) { }

    public DefaultRectangle(Rectangle rectangle)
    {
        Bounds = rectangle;
    }
}
