using RectanglePacker.Comparison;
using RectanglePacker.Events;
using RectanglePacker.Organisation;

namespace RectanglePacker;

public class BasePacker<T, R>
    where T : class, ITile<R>, new()
    where R : class, IRectangle
{
    protected readonly List<T> _tiles = [];
    protected readonly List<R> _rectangles = [];
    protected readonly List<R> _orphanedRectangles = [];

    public bool AllowEmptyPacking { get; set; }
    public int TotalRectangles => _rectangles.Count;
    public int TotalTiles => _tiles?.Count ?? 0;
    public int TotalSpace => TileWidth * TileHeight * MaximumTiles;
    public int TotalFreeSpace => GetTotalFreeSpace();
    public int TotalUsedSpace => GetTotalUsedSpace();
    public int MaximumTiles { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public PackingOptions Options { get; set; } = new();

    public IReadOnlyList<T> Tiles => _tiles;
    public IReadOnlyList<R> OrphanedRectangles => _orphanedRectangles;

    public event EventHandler<RectanglePositionEventArgs<T, R>> RectanglePositioned;

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
        _tiles.Clear();
    }

    public PackingResult<T, R> Pack()
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

        if (_rectangles.Count == 0 && !AllowEmptyPacking)
        {
            throw new ArgumentException("There are no rectangles to pack.");
        }

        int areaRequired = 0;
        foreach (R rectangle in _rectangles)
        {
            areaRequired += rectangle.Area;
        }
        
        if (areaRequired > TotalFreeSpace)
        {
            throw new InvalidOperationException(string.Format("There is not enough free space to pack the given rectangles (required: {0}, available: {1}).", areaRequired, TotalFreeSpace));
        }

        foreach (T tile in _tiles)
        {
            tile.PackingStarted();
        }

        PackingResult<T, R> results = new(this);
        results.BeginTimer();

        SortRectangles();

        _orphanedRectangles.Clear();

        List<R> queue = [.. _rectangles];

        if (Options.StartMethod != PackingStartMethod.FirstTile)
        {
            int tileStartIndex = Options.StartMethod == PackingStartMethod.EndTile ? _tiles.Count - 1 : _tiles.Count;
            foreach (R r in _rectangles)
            {
                if (Pack(r, tileStartIndex))
                {
                    queue.Remove(r);
                }
            }
        }

        foreach (R r in queue)
        {
            if (!Pack(r))
            {
                _orphanedRectangles.Add(r);
            }
        }

        results.EndTimer();
        return results;
    }

    protected void SortRectangles()
    {
        IComparer<R> comparer = Options.OrderMode switch
        {
            PackingOrderMode.Area => new AreaComparer<R>(),
            PackingOrderMode.Height => new HeightComparer<R>(),
            PackingOrderMode.Perimiter => new PerimiterComparer<R>(),
            _ => new WidthComparer<R>(),
        };
        _rectangles.Sort(comparer);

        if (Options.Order == PackingOrder.Descending)
        {
            _rectangles.Reverse();
        }

        if (Options.GroupMode == PackingGroupMode.Squares)
        {
            Dictionary<int, List<R>> squareMap = [];
            for (int i = _rectangles.Count - 1; i >= 0; i--)
            {
                R r = _rectangles[i];
                if (r.Bounds.Width == r.Bounds.Height)
                {
                    if (!squareMap.TryGetValue(r.Area, out List<R> value))
                    {
                        value = [];
                        squareMap[r.Area] = value;
                    }

                    value.Add(r);
                    _rectangles.RemoveAt(i);
                }
            }

            List<int> areaKeys = [.. squareMap.Keys];
            areaKeys.Sort();
            if (Options.Order == PackingOrder.Ascending)
            {
                areaKeys.Reverse();
            }
            foreach (int area in areaKeys)
            {
                _rectangles.InsertRange(0, squareMap[area]);
            }
        }
    }

    protected bool Pack(R rectangle, int startIndex = 0)
    {
        for (int i = startIndex; i < MaximumTiles; i++)
        {
            if (_tiles.Count == i)
            {
                AddTile();
            }

            T tile = _tiles[i];
            if (tile.Add(rectangle))
            {
                FireRectanglePositioned(rectangle, tile, i);
                return true;
            }
        }
        return false;
    }

    protected void AddTiles(uint count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            AddTile();
        }
    }

    protected T AddTile()
    {
        T newTile = new()
        {
            Index = _tiles.Count,
            Width = TileWidth,
            Height = TileHeight,
            FillMode = Options.FillMode
        };
        _tiles.Add(newTile);
        return newTile;
    }

    protected void FireRectanglePositioned(R rectangle, T tile, int tileIndex)
    {
        RectanglePositioned?.Invoke(this, new RectanglePositionEventArgs<T, R>
        {
            Rectangle = rectangle,
            Tile = tile,
            TileIndex = tileIndex
        });
    }

    protected int GetTotalFreeSpace()
    {
        int total = 0;
        for (int i = 0; i < MaximumTiles; i++)
        {
            if (i >= _tiles.Count)
            {
                total += TileWidth * TileHeight;
            }
            else
            {
                total += _tiles[i].FreeSpace;
            }
        }
        return total;
    }

    protected int GetTotalUsedSpace()
        => _tiles.Sum(t => t.UsedSpace);
}
