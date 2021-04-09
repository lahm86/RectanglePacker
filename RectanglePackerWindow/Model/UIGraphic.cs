using RectanglePacker;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RectanglePackerWindow.Model
{
    public class UIGraphic : IRectangle
    {
        private Image _image;
        private BitmapImage _bitmap;
        private int _area, _perimiter;

        public BitmapImage Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                _area = Width * Height;
                _perimiter = 2 * Width + 2 * Height;

                _image = new Image
                {
                    Width = Width,
                    Height = Height,
                    Source = _bitmap
                };
            }
        }

        public Image Image => _image;

        public int OriginalX { get; set; }
        public int OriginalY { get; set; }
        public int MappedX { get; set; }
        public int MappedY { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Area => _area;
        public int Perimiter => _perimiter;

        public UIGraphic(string filePath)
        : this(new BitmapImage(new Uri(filePath))) { }

        public UIGraphic(BitmapImage bitmap)
        {
            Width = (int)bitmap.Width;
            Height = (int)bitmap.Height;
            Bitmap = bitmap;
            OriginalX = OriginalY = 0;            
        }
    }
}