using System.Collections.Generic;
using System.Drawing;

namespace RectanglePacker
{
    public class Tile<R> where R : class, IRectangle
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int Area => Width * Height;
        public int FreeSpace => Area - UsedSpace;
        public int UsedSpace { get; private set; }

        public PackingFillMode FillMode { get; set; }

        public IReadOnlyList<R> Rectangles => _rectangles;

        private readonly List<R> _rectangles;

        private Region _occupiedRegion;

        public Tile()
        {
            _rectangles = new List<R>();
            UsedSpace = 0;
        }

        public bool Add(R rectangle)
        {
            if (FreeSpace < rectangle.Area)
            {
                return false;
            }

            if (_occupiedRegion == null)
            {
                InitialiseOccupiedRegion(rectangle);
                return true;
            }

            return FillMode == PackingFillMode.Horizontal ? AddHorizontally(rectangle) : AddVertically(rectangle);
        }

        private bool AddHorizontally(R rectangle)
        {
            int maxX = Width - rectangle.Width;
            int maxY = Height - rectangle.Height;

            Point p = new Point(0, 0);
            while (p.Y <= maxY)
            {
                while (p.X <= maxX)
                {
                    if (!_occupiedRegion.IsVisible(p) && Add(rectangle, p.X, p.Y))
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

        private bool AddVertically(R rectangle)
        {
            int maxX = Width - rectangle.Width;
            int maxY = Height - rectangle.Height;

            Point p = new Point(0, 0);
            while (p.X <= maxX)
            {
                while (p.Y <= maxY)
                {
                    if (!_occupiedRegion.IsVisible(p) && Add(rectangle, p.X, p.Y))
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

        public bool Add(R rectangle, int x, int y)
        {
            Rectangle mappedRect = new Rectangle(x, y, rectangle.Width, rectangle.Height);
            if (_occupiedRegion.IsVisible(mappedRect))
            {
                return false;
            }

            rectangle.MappedX = x;
            rectangle.MappedY = y;
            StoreRectangle(rectangle, mappedRect);

            return true;
        }

        private void InitialiseOccupiedRegion(R rectangle)
        {
            rectangle.MappedX = 0;
            rectangle.MappedY = 0;
            Rectangle mappedRect = new Rectangle(0, 0, rectangle.Width, rectangle.Height);
            _occupiedRegion = new Region(mappedRect);

            StoreRectangle(rectangle, null);
        }

        private void StoreRectangle(R rectangle, Rectangle? mappedRectangle)
        {
            _rectangles.Add(rectangle);
            UsedSpace += rectangle.Area;
            if (mappedRectangle.HasValue)
            {
                _occupiedRegion.Union(mappedRectangle.Value);
            }
        }
    }
}