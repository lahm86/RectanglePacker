using System;
using System.Collections.Generic;

namespace RectanglePacker
{
    public class Packer<R> where R : class, IRectangle
    {
        private List<Tile<R>> _tiles;
        private readonly List<R> _rectangles;

        public int TotalRectangles => _rectangles.Count;
        public int MaximumTiles { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public PackingFillMode FillMode { get; set; }
        public PackingGroupMode GroupMode { get; set; }
        public PackingOrderMode OrderMode { get; set; }
        public PackingOrder Order { get; set; }

        public IReadOnlyList<Tile<R>> Tiles => _tiles;

        public event EventHandler<RectanglePositionEventArgs<R>> RectanglePositioned;

        public Packer()
        {
            _rectangles = new List<R>();
            MaximumTiles = 0;
            TileWidth = 0;
            TileHeight = 0;
        }

        public void AddRectangle(R rectangle)
        {
            _rectangles.Add(rectangle);
        }

        public void AddRectangles(IEnumerable<R> rectangles)
        {
            _rectangles.AddRange(rectangles);
        }

        public void Reset()
        {
            _rectangles.Clear();
        }

        public PackingResult<R> Pack()
        {
            if (MaximumTiles <= 0)
            {
                throw new ArgumentException("Maximum number of tiles is <= 0 - packing cannot begin.");
            }

            if (TileWidth <= 0)
            {
                throw new ArgumentException("Tile width is <= 0 - packing cannot begin.");
            }

            if (TileHeight <= 0)
            {
                throw new ArgumentException("Tile height is <= 0 - packing cannot begin.");
            }

            if (_rectangles.Count == 0)
            {
                throw new ArgumentException("There are no rectangles to pack.");
            }

            PackingResult<R> results = new PackingResult<R>(this);
            results.BeginTimer();

            SortRectangles();

            _tiles = new List<Tile<R>>(MaximumTiles);

            foreach (R r in _rectangles)
            {
                if (!Pack(r))
                {
                    throw new Exception();
                }
            }

            results.EndTimer();
            return results;
        }

        private void SortRectangles()
        {
            IComparer<R> comparer;
            switch (OrderMode)
            {
                case PackingOrderMode.Area:
                    comparer = new AreaComparer<R>();
                    break;
                case PackingOrderMode.Height:
                    comparer = new HeightComparer<R>();
                    break;
                case PackingOrderMode.Perimiter:
                    comparer = new PerimiterComparer<R>();
                    break;
                case PackingOrderMode.Width:
                default:
                    comparer = new WidthComparer<R>();
                    break;
            }

            _rectangles.Sort(comparer);

            if (Order == PackingOrder.Descending)
            {
                _rectangles.Reverse();
            }

            if (GroupMode == PackingGroupMode.Squares)
            {
                Dictionary<int, List<R>> squareMap = new Dictionary<int, List<R>>();
                for (int i = _rectangles.Count - 1; i >= 0; i--)
                {
                    R r = _rectangles[i];
                    if (r.Width == r.Height)
                    {
                        if (!squareMap.ContainsKey(r.Area))
                        {
                            squareMap[r.Area] = new List<R>();
                        }
                        squareMap[r.Area].Add(r);
                        _rectangles.RemoveAt(i);
                    }
                }

                List<int> areaKeys = new List<int>(squareMap.Keys);
                areaKeys.Sort();
                if (Order == PackingOrder.Ascending)
                {
                    areaKeys.Reverse();
                }
                foreach (int area in areaKeys)
                {
                    _rectangles.InsertRange(0, squareMap[area]);
                }
            }
        }

        private bool Pack(R rectangle)
        {
            for (int i = 0; i < _tiles.Capacity; i++)
            {
                if (_tiles.Count == i)
                {
                    _tiles.Add(new Tile<R>
                    {
                        Width = TileWidth,
                        Height = TileHeight,
                        FillMode = FillMode
                    });
                }

                Tile<R> tile = _tiles[i];
                if (tile.Add(rectangle))
                {
                    FireRectanglePositioned(rectangle, tile, i);
                    return true;
                }
            }
            return false;
        }

        private void FireRectanglePositioned(R rectangle, Tile<R> tile, int tileIndex)
        {
            RectanglePositioned?.Invoke(this, new RectanglePositionEventArgs<R>
            {
                Rectangle = rectangle,
                Tile = tile,
                TileIndex = tileIndex
            });
        }
    }

    class AreaComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            return r1.Area.CompareTo(r2.Area);
        }
    }

    class HeightComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            return r1.Height.CompareTo(r2.Height);
        }
    }

    class PerimiterComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            return r1.Perimiter.CompareTo(r2.Perimiter);
        }
    }

    class WidthComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            return r1.Width.CompareTo(r2.Width);
        }
    }

    class SquareComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            bool b1 = r1.Width == r1.Height;
            bool b2 = r2.Width == r2.Height;
            if (b1 && b2)
            {
                return r1.Area.CompareTo(r2.Area);
            }

            return b1 ? -1 : 1;
        }
    }
}