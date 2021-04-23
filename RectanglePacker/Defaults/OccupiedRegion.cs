using System.Drawing;

namespace RectanglePacker.Defaults
{
    public class OccupiedRegion
    {
        private readonly int _width, _height;
        private readonly byte[] _data;

        public OccupiedRegion(int width, int height)
        {
            _width = width;
            _height = height;
            int capacity = width * height;
            _data = new byte[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _data[i] = 0;
            }
        }

        public Point GetFirstFreeVerticalPoint()
        {
            for (int col = 0; col < _width; col++)
            {
                for (int row = 0; row < _height; row++)
                {
                    if (Get(row, col) == 0)
                    {
                        return new Point(col, row);
                    }
                }
            }
            return new Point(0, 0);
        }

        public Point GetFirstFreeHorizontalPoint()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                if (_data[i] == 0)
                {
                    return IndexToPoint(i);
                }
            }
            return new Point(0, 0);
        }

        private Point IndexToPoint(int index)
        {
            int colDiff = index % _width;
            index -= colDiff;
            return new Point(colDiff, index / _width);
        }

        public bool CanFit(Rectangle r)
        {
            return CanFit(r.X, r.Y, r.Width, r.Height);
        }

        public bool CanFit(int x, int y, int w, int h)
        {
            int endX = x + w - 1;
            int endY = y + h - 1;
            if (!IsValid(endY, endX))
            {
                return false;
            }

            for (int row = y; row <= endY; row++)
            {
                for (int col = x; col <= endX; col++)
                {
                    if (Get(row, col) == 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsValid(int row, int col)
        {
            return _width * row + col < _data.Length;
        }

        public void Add(Rectangle r)
        {
            Add(r.X, r.Y, r.Width, r.Height);
        }

        public void Add(int x, int y, int w, int h)
        {
            Set(x, y, w, h, 1);
        }

        public void Remove(Rectangle r)
        {
            Remove(r.X, r.Y, r.Width, r.Height);
        }

        public void Remove(int x, int y, int w, int h)
        {
            Set(x, y, w, h, 0);
        }

        private void Set(int x, int y, int w, int h, byte value)
        {
            int endX = x + w - 1;
            int endY = y + h - 1;

            for (int row = y; row <= endY; row++)
            {
                for (int col = x; col <= endX; col++)
                {
                    Set(row, col, value);
                }
            }
        }

        private int Get(int row, int col)
        {
            return _data[_width * row + col];
        }

        private void Set(int row, int col, byte value)
        {
            _data[_width * row + col] = value;
        }
    }
}