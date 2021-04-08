using Newtonsoft.Json;
using RectanglePacker;
using RectanglePackerWindow.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RectanglePackerWindow.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Brush[] _allBrushes = LoadBrushes();
        private static readonly Random _rand = new Random();

        private List<Size> _sizes;
        private List<UIRectangle> _rectangles;
        private Packer<UIRectangle> _packer;
        private Dictionary<int, Canvas> _tileMap;
        private Dictionary<int, Rectangle> _rectMap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _sizes = JsonConvert.DeserializeObject<List<Size>>(File.ReadAllText(@"SampleData\SampleSizes.json"));
            Reset();
        }

        private void RectanglesButton_Click(object sender, RoutedEventArgs e)
        {
            RectangleGeneratorWindow rgw = new RectangleGeneratorWindow();
            if (rgw.ShowDialog() ?? false)
            {

            }
        }

        private void Reset()
        {
            _canvasContainer.Children.Clear();
            _unsortedPanel.Children.Clear();
            _rectangles = new List<UIRectangle>();
            _rectMap = new Dictionary<int, Rectangle>();

            _sizes.Randomise(_rand);

            Thickness m = new Thickness(3, 3, 3, 3);
            foreach (Size s in _sizes)
            {
                UIRectangle r = new UIRectangle(0, 0, (int)s.Width, (int)s.Height);
                _rectangles.Add(r);

                Rectangle r2 = new Rectangle
                {
                    Width = r.Width,
                    Height = r.Height,
                    Fill = _allBrushes[_rand.Next(0, _allBrushes.Length)],
                    Margin = m
                };
                _unsortedPanel.Children.Add(r2);
                _rectMap[r.ID] = r2;
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();

            _packer = new Packer<UIRectangle>
            {
                TileWidth = _widthSpinner.Value,
                TileHeight = _heightSpinner.Value,
                MaximumTiles = _tilesSpinner.Value,
                FillMode = (PackingFillMode)_packingModeCombo.SelectedIndex,
                OrderMode = (PackingOrderMode)_sortModeCombo.SelectedIndex,
                Order = (PackingOrder)_sortOrderCombo.SelectedIndex,
                GroupMode = (PackingGroupMode)_groupByCombo.SelectedIndex
            };
            _packer.AddRectangles(_rectangles);

            _packer.RectanglePositioned += Packer_RectanglePositioned;

            _tileMap = new Dictionary<int, Canvas>();

            new Thread(Run).Start();
        }

        private void Run()
        {
            _packer.Pack();
        }

        private void Packer_RectanglePositioned(object sender, RectanglePositionEventArgs<UIRectangle> e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => PaintRectangle(e)));
            }
            else
            {
                PaintRectangle(e);
            }
        }

        private void PaintRectangle(RectanglePositionEventArgs<UIRectangle> e)
        {
            if (!_tileMap.ContainsKey(e.TileIndex))
            {
                _tileMap[e.TileIndex] = new Canvas
                {
                    Width = e.Tile.Width,
                    Height = e.Tile.Height,
                    Margin = new Thickness(20, 20, 20, 20)
                };
                _canvasContainer.Children.Add(_tileMap[e.TileIndex]);
            }

            //Rectangle r = new Rectangle
            //{
            //    Width = e.Rectangle.Width,
            //    Height = e.Rectangle.Height,
            //    Fill = _allBrushes[_rand.Next(0, _allBrushes.Length)]
            //};

            Rectangle r = _rectMap[e.Rectangle.ID];
            _unsortedPanel.Children.Remove(r);
            r.Margin = new Thickness(e.Rectangle.MappedX, e.Rectangle.MappedY, 0, 0);

            _tileMap[e.TileIndex].Children.Add(r);
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