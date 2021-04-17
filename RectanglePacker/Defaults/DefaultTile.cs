using RectanglePacker.Organisation;
using System.Collections.Generic;
using System.Drawing;

namespace RectanglePacker.Defaults
{
    public class DefaultTile<R> : ITile<R> where R : class, IRectangle
    {
        public int Index { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int Area => Width * Height;
        public int FreeSpace => Area - UsedSpace;
        public int UsedSpace { get; private set; }
        
        public PackingFillMode FillMode { get; set; }

        public IReadOnlyList<R> Rectangles => _rectangles;

        protected readonly List<R> _rectangles;

        protected Region _occupiedRegion;

        public DefaultTile()
        {
            _rectangles = new List<R>();
            UsedSpace = 0;
        }

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

        protected bool AddVertically(R rectangle)
        {
            int maxX = Width - rectangle.Bounds.Width;
            int maxY = Height - rectangle.Bounds.Height;

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

        protected virtual bool Add(R rectangle, int x, int y)
        {
            Rectangle mappedRect = new Rectangle(x, y, rectangle.Bounds.Width, rectangle.Bounds.Height);
            if (_occupiedRegion == null)
            {
                _occupiedRegion = new Region(mappedRect);
            }
            else if (_occupiedRegion.IsVisible(mappedRect))
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
                _occupiedRegion.Union(mappedRectangle);
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
                    _occupiedRegion.Exclude(new Rectangle(rectangle.MappedX, rectangle.MappedY, rectangle.Bounds.Width, rectangle.Bounds.Height));
                }
                return true;
            }
            return false;
        }

        public Rectangle GetOccupiedRegion(Graphics g)
        {
            RectangleF rect = _occupiedRegion.GetBounds(g);
            return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
        }
    }
}