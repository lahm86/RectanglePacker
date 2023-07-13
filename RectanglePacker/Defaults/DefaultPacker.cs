namespace RectanglePacker.Defaults;

public class DefaultPacker : AbstractPacker<DefaultTile<DefaultRectangle>, DefaultRectangle>
{
    protected override DefaultTile<DefaultRectangle> CreateTile()
    {
        return new DefaultTile<DefaultRectangle>();
    }
}
