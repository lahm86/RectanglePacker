using RectanglePacker;
using System.Drawing;

namespace RectanglePackerWindow.Model
{
    public class UIRectangle : IRectangle
    {
        private static int _id = 0;

        public int ID { get; private set; }

        private Rectangle _rect;
        private int _area, _perimiter;

        public Rectangle Rectangle
        {
            get => _rect;
            set
            {
                _rect = value;
                _area = Width * Height;
                _perimiter = 2 * Width + 2 * Height;
            }
        }

        public int OriginalX => Rectangle.X;
        public int OriginalY => Rectangle.Y;
        public int Width => Rectangle.Width;
        public int Height => Rectangle.Height;

        public int Area => _area;
        public int Perimiter => _perimiter;
        public int MappedX { get; set; }
        public int MappedY { get; set; }

        public UIRectangle(int x, int y, int width, int height)
        {
            ID = _id++;
            Rectangle = new Rectangle(x, y, width, height);
        }
    }
}