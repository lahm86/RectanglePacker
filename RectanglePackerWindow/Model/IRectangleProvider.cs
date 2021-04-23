using RectanglePacker;
using RectanglePacker.Organisation;
using System;
using System.Collections.Generic;
using System.Windows;

namespace RectanglePackerWindow.Model
{
    public interface IRectangleProvider
    {
        List<IRectangle> Rectangles { get; }
        int GeneratedTileCount { get; }
        event EventHandler<RectanglePositionedEventArgs> RectanglePositioned;
        void LoadRectangles(string filePath, bool shuffle = false);
        void LoadRectangles(List<string> filePaths, bool shuffle = false);
        void LoadRectangles(List<Size> sizes, bool shuffle = false);
        void ResetRectangles();
        void Pack(int tileWidth, int tileHeight, int maxTiles, PackingFillMode fillMode, PackingOrderMode orderMode, PackingOrder order, PackingGroupMode groupMode);
        TimeSpan GetProcessingTime();
    }
}