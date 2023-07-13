using RectanglePacker.Organisation;

namespace RectanglePacker;

public interface ITile<R>
    where R : class, IRectangle
{
    int Index { get; set; }
    int Width { get; set; }
    int Height { get; set; }

    int Area { get; }
    int FreeSpace { get; }
    int UsedSpace { get; }

    PackingFillMode FillMode { get; set; }

    bool Add(R rectangle);
    bool Remove(R rectangle);
    void PackingStarted();
}
