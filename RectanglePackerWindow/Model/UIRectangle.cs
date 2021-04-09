using RectanglePacker;
using System.Windows.Shapes;

namespace RectanglePackerWindow.Model
{
    public class UIRectangle : IRectangle
    {
        private Rectangle _rect;
        private int _area, _perimiter;

        public Rectangle RectangleShape
        {
            get => _rect;
            set
            {
                _rect = value;
                _area = Width * Height;
                _perimiter = 2 * Width + 2 * Height;
            }
        }

        public int OriginalX { get; private set; }
        public int OriginalY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Area => _area;
        public int Perimiter => _perimiter;
        public int MappedX { get; set; }
        public int MappedY { get; set; }

        public UIRectangle(int x, int y, int width, int height)
        {
            OriginalX = x;
            OriginalY = y;
            Width = width;
            Height = height;
            RectangleShape = new Rectangle
            {
                Width = width,
                Height = height
            };
        }
    }
}