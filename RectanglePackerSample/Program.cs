using Newtonsoft.Json;
using RectanglePacker.Defaults;
using RectanglePacker.Events;
using RectanglePacker.Organisation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace RectanglePackerSample
{
    class Program
    {
        private static PackerSample _sample;
        private static StringBuilder _resultData;

        static void Main()
        {
            _resultData = new StringBuilder();
            _resultData.Append("# Rectangles,Fill Mode,Packing Order Mode,Packing Order,Group Mode,Time,Tiles,Space Occupied,Tile Occupation");

            Console.WriteLine("Choose a sample mode [0 or 1]");
            Console.WriteLine("[0] Run all possible configurations");
            Console.WriteLine("[1] Enter configuration manually");
            int mode = ReadInt(0, 1);

            Console.Write("Reading sample rectangles...");
            List<DefaultRectangle> rectangles = new List<DefaultRectangle>();
            foreach (Size s in JsonConvert.DeserializeObject<Size[]>(File.ReadAllText("SampleSizes.json")))
            {
                rectangles.Add(new DefaultRectangle(0, 0, s.Width, s.Height));
            }
            Console.WriteLine("{0} rectangles loaded", rectangles.Count);

            _sample = new PackerSample
            {
                Rectangles = rectangles,
                DrawBitmaps = true
            };

            _sample.Packer.RectanglePositioned += Packer_RectanglePositioned;

            if (mode == 0)
            {
                RunAllConfigurations();
            }
            if (mode == 1)
            {
                RunSingleConfiguration();
            }


            string outputPath = "Results_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".csv";
            File.WriteAllText(outputPath, _resultData.ToString());

            Console.WriteLine();
            Console.WriteLine("Packing finished");
            Console.WriteLine("CSV results stored in {0}", outputPath);
            Console.WriteLine("Bitmap results stored in TileOutput");

            Console.Read();
        }

        private static void Packer_RectanglePositioned(object sender, RectanglePositionEventArgs<DefaultTile<DefaultRectangle>, DefaultRectangle> e)
        {
            //Console.WriteLine("Tile {0} => [{1}, {2}, {3}, {4}]", e.TileIndex, e.Rectangle.MappedX, e.Rectangle.MappedY, e.Rectangle.Width, e.Rectangle.Height);
        }

        private static int ReadInt(int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                string val = Console.ReadLine();
                if (int.TryParse(val, out int i))
                {
                    if (i >= min && i <= max)
                    {
                        return i;
                    }
                }
            }
        }

        private static void RunAllConfigurations()
        {
            PackingFillMode[] fillModes = (PackingFillMode[])Enum.GetValues(typeof(PackingFillMode));
            PackingGroupMode[] groupModes = (PackingGroupMode[])Enum.GetValues(typeof(PackingGroupMode));
            PackingOrderMode[] orderModes = (PackingOrderMode[])Enum.GetValues(typeof(PackingOrderMode));
            PackingOrder[] orders = (PackingOrder[])Enum.GetValues(typeof(PackingOrder));

            foreach (PackingFillMode fillMode in fillModes)
            {
                foreach (PackingOrderMode orderMode in orderModes)
                {
                    foreach (PackingOrder order in orders)
                    {
                        foreach (PackingGroupMode groupMode in groupModes)
                        {
                            ConvertResults(Pack(fillMode, orderMode, order, groupMode));
                        }
                    }
                }
            }
        }

        private static void RunSingleConfiguration()
        {
            Console.WriteLine();
            Console.WriteLine("Choose a packing fill mode [0 or 1]");
            Console.WriteLine("[0] Pack horizontally from top left");
            Console.WriteLine("[1] Pack vertically from top left");
            PackingFillMode fillMode = (PackingFillMode)ReadInt(0, 1);

            Console.WriteLine();
            Console.WriteLine("Choose a rectangle sorting mode [0, 1, 2 or 3]");
            Console.WriteLine("[0] Sort by Width");
            Console.WriteLine("[1] Sort by Height");
            Console.WriteLine("[2] Sort by Area");
            Console.WriteLine("[3] Sort by Perimiter");
            PackingOrderMode orderMode = (PackingOrderMode)ReadInt(0, 3);

            Console.WriteLine();
            Console.WriteLine("Choose the rectangle sorting order [0 or 1]");
            Console.WriteLine("[0] Ascending");
            Console.WriteLine("[1] Descending");
            PackingOrder order = (PackingOrder)ReadInt(0, 1);

            Console.WriteLine();
            Console.WriteLine("Choose the grouping mode [0 or 1]");
            Console.WriteLine("[0] None");
            Console.WriteLine("[1] Group by squares");
            PackingGroupMode groupMode = (PackingGroupMode)ReadInt(0, 1);

            PackingResult<DefaultTile<DefaultRectangle>, DefaultRectangle> results = Pack(fillMode, orderMode, order, groupMode);
            ConvertResults(results);
        }

        private static PackingResult<DefaultTile<DefaultRectangle>, DefaultRectangle> Pack(PackingFillMode fillMode, PackingOrderMode orderMode, PackingOrder order, PackingGroupMode groupMode)
        {
            _sample.Packer.TileWidth = 256;
            _sample.Packer.TileHeight = 256;
            _sample.Packer.MaximumTiles = 16;
            _sample.Packer.Options = new PackingOptions
            {
                FillMode = fillMode,
                OrderMode = orderMode,
                Order = order,
                GroupMode = groupMode
            };

            Console.WriteLine();
            Console.WriteLine("Packing {0}ly ordered by {1} {2}, grouped by {3}", fillMode, orderMode, order, groupMode);
            return _sample.Pack();
        }

        private static void ConvertResults(PackingResult<DefaultTile<DefaultRectangle>, DefaultRectangle> results)
        {
            _resultData.AppendLine();

            _resultData.Append(results.Packer.TotalRectangles).Append(",");
            _resultData.Append(results.Packer.Options.FillMode).Append(",");
            _resultData.Append(results.Packer.Options.OrderMode).Append(",");
            _resultData.Append(results.Packer.Options.Order).Append(",");
            _resultData.Append(results.Packer.Options.GroupMode).Append(",");
            _resultData.Append(results.PackingTime.TotalSeconds).Append(",");
            _resultData.Append(results.UsedTileCount).Append(",");
            _resultData.Append(results.TotalSpaceOccupation).Append("%");

            IReadOnlyList<DefaultTile<DefaultRectangle>> tiles = results.Packer.Tiles;
            foreach (DefaultTile<DefaultRectangle> tile in tiles)
            {
                _resultData.Append(",").Append(Math.Round(100 * (double)tile.UsedSpace / tile.Area, 2)).Append("%");
            }
        }
    }
}