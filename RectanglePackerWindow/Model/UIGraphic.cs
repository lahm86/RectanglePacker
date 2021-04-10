using RectanglePacker.Defaults;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RectanglePackerWindow.Model
{
    public class UIGraphic : DefaultRectangle
    {
        private Image _image;
        private BitmapImage _bitmap;

        public BitmapImage Bitmap
        {
            get => _bitmap;
            set
            {
                _bitmap = value;
                _image = new Image
                {
                    Width = Width,
                    Height = Height,
                    Source = _bitmap
                };
            }
        }

        public Image Image => _image;

        public UIGraphic(string filePath)
            : this(new BitmapImage(new Uri(filePath))) { }

        public UIGraphic(BitmapImage bitmap)
            :base(0, 0, (int)bitmap.Width, (int)bitmap.Height)
        {
            Bitmap = bitmap;
        }
    }
}