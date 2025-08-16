using RectanglePacker.Organisation;
using System.Drawing;

namespace RectanglePacker.Defaults;

public class DefaultTile<R> : ITile<R>
    where R : class, IRectangle
{
    public int Index { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public int Area => Width * Height;
    public int FreeSpace => Area - UsedSpace;
    public int UsedSpace { get; private set; }
    
    public PackingFillMode FillMode { get; set; }

    public IReadOnlyList<R> Rectangles => _rectangles;

    protected readonly List<R> _rectangles = [];

    protected OccupiedRegion _occupiedRegion;

    protected bool _allowOverlapping;

    public virtual void PackingStarted() { }

    public bool Add(R rectangle)
    {
        if (FreeSpace < rectangle.Area)
        {
            return false;
        }

        if (UsedSpace == 0)
        {
            return Add(rectangle, 0, 0);
        }

        return FillMode == PackingFillMode.Horizontal ? AddHorizontally(rectangle) : AddVertically(rectangle);
    }

    protected bool AddHorizontally(R rectangle)
    {
        int maxX = Width - rectangle.Bounds.Width;
        int maxY = Height - rectangle.Bounds.Height;

        Point p = _occupiedRegion.GetFirstFreeHorizontalPoint();
        while (p.Y <= maxY)
        {
            while (p.X <= maxX)
            {
                if (Add(rectangle, p.X, p.Y))
                {
                    return true;
                }
                ++p.X;
            }
            ++p.Y;
            p.X = 0;
        }

        return false;
    }

    protected bool AddVertically(R rectangle)
    {
        int maxX = Width - rectangle.Bounds.Width;
        int maxY = Height - rectangle.Bounds.Height;

        Point p = _occupiedRegion.GetFirstFreeVerticalPoint();
        while (p.X <= maxX)
        {
            while (p.Y <= maxY)
            {
                if (Add(rectangle, p.X, p.Y))
                {
                    return true;
                }
                ++p.Y;
            }
            ++p.X;
            p.Y = 0;
        }

        return false;
    }

    protected bool Add(R rectangle, Point point)
    {
        return Add(rectangle, point.X, point.Y);
    }

    protected virtual bool Add(R rectangle, int x, int y)
    {
        Rectangle mappedRect = new(x, y, rectangle.Bounds.Width, rectangle.Bounds.Height);
        if (_occupiedRegion == null)
        {
            _occupiedRegion = new OccupiedRegion(Width, Height);
            _occupiedRegion.Add(mappedRect);
        }
        else if (!_allowOverlapping && !_occupiedRegion.CanFit(mappedRect))
        {
            return false;
        }

        rectangle.MappedX = x;
        rectangle.MappedY = y;
        StoreRectangle(rectangle, mappedRect);

        return true;
    }

    protected void StoreRectangle(R rectangle, Rectangle mappedRectangle)
    {
        _rectangles.Add(rectangle);
        UsedSpace += rectangle.Area;
        if (_rectangles.Count > 1)
        {
            _occupiedRegion.Add(mappedRectangle);
        }
    }

    public virtual bool Remove(R rectangle)
    {
        if (_rectangles.Remove(rectangle))
        {
            UsedSpace -= rectangle.Area;
            if (_rectangles.Count == 0)
            {
                _occupiedRegion = null;
            }
            else
            {
                _occupiedRegion.Remove(new Rectangle(rectangle.MappedX, rectangle.MappedY, rectangle.Bounds.Width, rectangle.Bounds.Height));
            }
            return true;
        }
        return false;
    }

    public Rectangle GetOccupiedRegion()
    {
        // Get the segment with the biggest x value and add its width; get the segment with the biggest y value and add its height.
        // Create a new rectangle from 0,0 to the max point. This makes sure that while whitespace is clipped, required whitespace 
        // within boundary segments is retained.

        int maxX = 0, maxY = 0;
        foreach (R r in _rectangles)
        {
            maxX = Math.Max(maxX, r.MappedX + r.Bounds.Width);
            maxY = Math.Max(maxY, r.MappedY + r.Bounds.Height);
        }

        return new Rectangle(0, 0, maxX, maxY);
    }
}
