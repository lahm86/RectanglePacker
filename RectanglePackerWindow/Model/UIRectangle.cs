using RectanglePacker;
using RectanglePacker.Defaults;
using System.Windows.Shapes;
using D = System.Drawing;

namespace RectanglePackerWindow.Model
{
    public class UIRectangle : DefaultRectangle
    {
        //private Rectangle _rect;
        //private int _area, _perimiter;

        public Rectangle RectangleShape { get; set; }
        /*{
            get => _rect;
            set
            {
                _rect = value;
                _area = Width * Height;
                _perimiter = 2 * Width + 2 * Height;
            }
        }

        public D.Rectangle Bounds { get; private set; }
        public int Width => Bounds.Width;
        public int Height => Bounds.Height;

        public int Area => _area;
        public int Perimiter => _perimiter;
        public int MappedX { get; set; }
        public int MappedY { get; set; }*/

        public UIRectangle(int x, int y, int width, int height)
            :base(x, y, width, height)
        {
            RectangleShape = new Rectangle
            {
                Width = width,
                Height = height
            };
        }
    }
}