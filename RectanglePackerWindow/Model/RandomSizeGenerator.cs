using System;
using System.Collections.Generic;
using System.Windows;

namespace RectanglePackerWindow.Model
{
    public class RandomSizeGenerator
    {
        private Random _rand;

        public int Count { get; set; }
        public int MinimimWidth { get; set; }
        public int MaximimWidth { get; set; }
        public int MinimimHeight { get; set; }
        public int MaximimHeight { get; set; }
        public bool SquaresOnly { get; set; }

        public List<Size> Generate()
        {
            _rand = new Random();
            List<Size> sizes = new List<Size>(Count);
            while (sizes.Count < Count)
            {
                sizes.Add(GenerateSize());
            }
            return sizes;
        }

        private Size GenerateSize()
        {
            int width = _rand.Next(MinimimWidth, MaximimWidth + 1);
            int height = SquaresOnly ? width : _rand.Next(MinimimHeight, MaximimHeight + 1);
            return new Size(width, height);
        }
    }
}