namespace RectanglePacker.Events;

public class PackingResult<T, R>
    where T : class, ITile<R>, new()
    where R : class, IRectangle
{
    public BasePacker<T, R> Packer { get; private set; }
    public double TotalSpaceOccupation { get; private set; }
    public int UsedTileCount => Packer.Tiles.Count;
    public int OrphanCount => Packer.OrphanedRectangles.Count;
    public TimeSpan PackingTime { get; private set; }

    private DateTime _startTime;

    internal PackingResult(BasePacker<T, R> packer)
    {
        Packer = packer;
    }

    internal void BeginTimer()
    {
        _startTime = DateTime.Now;
    }

    internal void EndTimer()
    {
        PackingTime = DateTime.Now.Subtract(_startTime);
        int spaceUsed = 0;
        IReadOnlyList<T> tiles = Packer.Tiles;
        for (int i = 0; i < tiles.Count; i++)
        {
            T tile = tiles[i];
            spaceUsed += tile.UsedSpace;
        }
        TotalSpaceOccupation = Math.Round(100 * (double)spaceUsed / (tiles[0].Area * tiles.Count), 2);
    }
}
