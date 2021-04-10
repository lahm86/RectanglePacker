using RectanglePacker;
using RectanglePacker.Defaults;

namespace RectanglePackerWindow.Model
{
    public class UIGraphicPacker : AbstractPacker<DefaultTile<UIGraphic>, UIGraphic>
    {
        protected override DefaultTile<UIGraphic> CreateTile()
        {
            return new DefaultTile<UIGraphic>();
        }
    }
}