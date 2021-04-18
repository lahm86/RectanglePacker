using Newtonsoft.Json;
using RectanglePacker;
using RectanglePacker.Defaults;
using RectanglePacker.Events;
using RectanglePacker.Organisation;
using RectanglePackerWindow.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace RectanglePackerWindow.Model
{
    public class UIRectangleProvider : IRectangleProvider
    {
        private static readonly Brush[] _allBrushes = LoadBrushes();
        private static readonly Random _rand = new Random();
        private static readonly Thickness _uiRectangleMargin = new Thickness(3, 3, 3, 3);

        public int GeneratedTileCount => _packer == null ? 0 : _packer.TotalTiles;

        private List<UIRectangle> _rectangles;
        public List<IRectangle> Rectangles => _rectangles.Cast<IRectangle>().ToList();

        private UIRectanglePacker _packer;

        public event EventHandler<RectanglePositionedEventArgs> RectanglePositioned;

        public void LoadRectangles(List<string> filePaths, bool shuffle = false)
        {
            _rectangles = new List<UIRectangle>();
            foreach (string filePath in filePaths)
            {
                _rectangles.AddRange(BuildRectangles(JsonConvert.DeserializeObject<List<Size>>(File.ReadAllText(filePath)), shuffle));
            }
        }

        public void LoadRectangles(string jsonFilePath, bool shuffle = false)
        {
            _rectangles = BuildRectangles(JsonConvert.DeserializeObject<List<Size>>(File.ReadAllText(jsonFilePath)), shuffle);
        }

        public void LoadRectangles(List<Size> sizes, bool shuffle = false)
        {
            _rectangles = BuildRectangles(sizes, shuffle);
        }

        private List<UIRectangle> BuildRectangles(List<Size> sizes, bool shuffle)
        {
            List<UIRectangle> rectangles = new List<UIRectangle>();
            foreach (Size s in sizes)
            {
                UIRectangle r = new UIRectangle(0, 0, (int)s.Width, (int)s.Height);
                r.RectangleShape.Fill = _allBrushes[_rand.Next(0, _allBrushes.Length)];
                r.RectangleShape.Margin = _uiRectangleMargin;
                rectangles.Add(r);
            }

            if (shuffle)
            {
                rectangles.Randomise(_rand);
            }

            return rectangles;
        }

        public void ResetRectangles()
        {
            foreach (UIRectangle rectangle in _rectangles)
            {
                rectangle.RectangleShape.Margin = _uiRectangleMargin;
            }
        }

        public void Pack(int tileWidth, int tileHeight, int maxTiles, PackingFillMode fillMode, PackingOrderMode orderMode, PackingOrder order, PackingGroupMode groupMode)
        {
            _packer = new UIRectanglePacker
            {
                TileWidth = tileWidth,
                TileHeight = tileHeight,
                MaximumTiles = maxTiles,
                Options = new PackingOptions
                {
                    FillMode = fillMode,
                    OrderMode = orderMode,
                    Order = order,
                    GroupMode = groupMode
                }
            };
            _packer.AddRectangles(_rectangles);
            _packer.RectanglePositioned += Packer_RectanglePositioned;
            PackingResult<DefaultTile<UIRectangle>, UIRectangle> result = _packer.Pack();
            if (result.OrphanCount > 0)
            {
                throw new Exception("At least one rectangle could not be positioned.");
            }
        }

        private void Packer_RectanglePositioned(object sender, RectanglePositionEventArgs<DefaultTile<UIRectangle>, UIRectangle> e)
        {
            RectanglePositioned?.Invoke(this, new RectanglePositionedEventArgs
            {
                Rectangle = e.Rectangle,
                TileIndex = e.TileIndex,
                TileWidth = e.Tile.Width,
                TileHeight = e.Tile.Height
            });
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