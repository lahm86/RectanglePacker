using RectanglePacker.Defaults;
using RectanglePacker.Events;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace RectanglePackerSample
{
    public class PackerSample
    {
        private static readonly Brush[] _allBrushes = LoadBrushes();
        private static readonly Random _rand = new Random();

        public List<DefaultRectangle> Rectangles { get; set; }
        public DefaultPacker Packer { get; private set; }
        public bool DrawBitmaps { get; set; }
        private const string _bitmapDirectory = "TileOutput";

        public PackerSample()
        {
            Packer = new DefaultPacker();
            DrawBitmaps = true;

            if (Directory.Exists(_bitmapDirectory))
            {
                Directory.Delete(_bitmapDirectory, true);
            }
            Directory.CreateDirectory(_bitmapDirectory);
        }

        public PackingResult<DefaultTile<DefaultRectangle>, DefaultRectangle> Pack()
        {
            Packer.Reset();
            Packer.AddRectangles(Rectangles);
            PackingResult<DefaultTile<DefaultRectangle>, DefaultRectangle> results = Packer.Pack();

            if (DrawBitmaps)
            {
                DrawBitmapOutputs();
            }

            return results;
        }

        private void DrawBitmapOutputs()
        {
            IReadOnlyList<DefaultTile<DefaultRectangle>> tiles = Packer.Tiles;
            for (int i = 0; i < tiles.Count; i++)
            {
                DefaultTile<DefaultRectangle> tile = tiles[i];
                using (Bitmap bmp = new Bitmap(tile.Width, tile.Height, PixelFormat.Format32bppArgb))
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    foreach (DefaultRectangle rect in tile.Rectangles)
                    {
                        Rectangle r = new Rectangle(rect.MappedX, rect.MappedY, rect.Width, rect.Height);
                        g.FillRectangle(_allBrushes[_rand.Next(0, _allBrushes.Length)], r);
                    }
                    bmp.Save(string.Format(@"{0}\{1}{2}{3}{4}_Tile{5}.png", _bitmapDirectory, Packer.FillMode, Packer.OrderMode, Packer.Order, Packer.GroupMode, i), ImageFormat.Png);
                }
            }
        }

        private static Brush[] LoadBrushes()
        {
            return new List<Brush>
            {
                Brushes.Aqua,
                Brushes.Aquamarine,
                Brushes.Bisque,
                Brushes.Black,
                Brushes.Blue,
                Brushes.BlueViolet,
                Brushes.Brown,
                Brushes.CadetBlue,
                Brushes.Chartreuse,
                Brushes.Chocolate,
                Brushes.Coral,
                Brushes.CornflowerBlue,
                Brushes.Crimson,
                Brushes.DarkBlue,
                Brushes.DarkCyan,
                Brushes.DarkGoldenrod,
                Brushes.DarkOrange,
                Brushes.DarkRed,
                Brushes.DarkSalmon,
                Brushes.DarkSeaGreen,
                Brushes.DarkSlateBlue,
                Brushes.DarkSlateGray,
                Brushes.DeepPink,
                Brushes.DeepSkyBlue,
                Brushes.ForestGreen,
                Brushes.Gold,
                Brushes.Green,
                Brushes.HotPink,
                Brushes.LawnGreen,
                Brushes.MidnightBlue,
                Brushes.Navy,
                Brushes.Olive,
                Brushes.Orange,
                Brushes.OrangeRed,
                Brushes.Orchid,
                Brushes.Pink,
                Brushes.Purple,
                Brushes.Red,
                Brushes.Teal,
                Brushes.Yellow
            }.ToArray();
        }
    }
}