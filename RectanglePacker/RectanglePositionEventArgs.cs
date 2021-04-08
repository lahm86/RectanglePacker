using System;

namespace RectanglePacker
{
    public class RectanglePositionEventArgs<R> : EventArgs where R : class, IRectangle
    {
        public R Rectangle { get; internal set; }
        public Tile<R> Tile { get; internal set; }
        public int TileIndex { get; internal set; }
    }
}