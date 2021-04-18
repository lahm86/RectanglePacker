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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using D = System.Drawing;

namespace RectanglePackerWindow.Model
{
    public class UIGraphicProvider : IRectangleProvider
    {
        private static readonly Thickness _uiRectangleMargin = new Thickness(3, 3, 3, 3);
        private static readonly Random _rand = new Random();

        public int GeneratedTileCount => _packer == null ? 0 : _packer.Tiles.Count;

        private List<UIGraphic> _rectangles;
        public List<IRectangle> Rectangles => _rectangles.Cast<IRectangle>().ToList();

        private UIGraphicPacker _packer;

        public event EventHandler<RectanglePositionedEventArgs> RectanglePositioned;

        public void LoadRectangles(string filePath, bool shuffle = false)
        {
            LoadRectangles(JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(filePath)), shuffle);
        }

        public void LoadRectangles(DirectoryInfo directory, bool shuffle = false)
        {
            List<FileInfo> files = directory.GetFilteredFiles(new string[] { "*.bmp", "*.gif", "*.jpg", "*.jpeg", "*.png" });
            List<string> filePaths = new List<string>(files.Count);
            files.ForEach(f => filePaths.Add(f.FullName));
            LoadRectangles(filePaths, shuffle);
        }

        public void LoadRectangles(List<string> filePaths, bool shuffle = false)
        {
            _rectangles = new List<UIGraphic>();
            foreach (string filePath in filePaths)
            {
                UIGraphic uig = new UIGraphic(filePath);                
                uig.Image.Margin = _uiRectangleMargin;
                _rectangles.Add(uig);
            }

            if (shuffle)
            {
                _rectangles.Randomise(_rand);
            }
        }

        public void LoadRectangles(List<Size> sizes, bool shuffle = false)
        {
            throw new NotImplementedException();
        }

        public void ResetRectangles()
        {
            foreach (UIGraphic rectangle in _rectangles)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(rectangle.Image);
                if (parent != null && parent is Panel panel)
                {
                    panel.Children.Remove(rectangle.Image);
                }
                rectangle.Image.Margin = _uiRectangleMargin;
            }
        }

        public void Pack(int tileWidth, int tileHeight, int maxTiles, PackingFillMode fillMode, PackingOrderMode orderMode, PackingOrder order, PackingGroupMode groupMode)
        {
            _packer = new UIGraphicPacker
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
            PackingResult<DefaultTile<UIGraphic>, UIGraphic> result = _packer.Pack();
            if (result.OrphanCount > 0)
            {
                throw new Exception("At least one rectangle could not be positioned.");
            }
        }

        private void Packer_RectanglePositioned(object sender, RectanglePositionEventArgs<DefaultTile<UIGraphic>, UIGraphic> e)
        {
            RectanglePositioned?.Invoke(this, new RectanglePositionedEventArgs
            {
                Rectangle = e.Rectangle,
                TileIndex = e.TileIndex,
                TileWidth = e.Tile.Width,
                TileHeight = e.Tile.Height
            });
        }
    }
}