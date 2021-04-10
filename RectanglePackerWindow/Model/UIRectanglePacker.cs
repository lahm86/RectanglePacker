using RectanglePacker;
using RectanglePacker.Defaults;

namespace RectanglePackerWindow.Model
{
    public class UIRectanglePacker : AbstractPacker<DefaultTile<UIRectangle>, UIRectangle>
    {
        protected override DefaultTile<UIRectangle> CreateTile()
        {
            return new DefaultTile<UIRectangle>();
        }
    }
}