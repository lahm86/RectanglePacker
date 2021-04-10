using System;

namespace RectanglePacker.Events
{
    public class RectanglePositionEventArgs<T, R> : EventArgs where T : class, ITile<R> where R : class, IRectangle
    {
        public R Rectangle { get; internal set; }
        public T Tile { get; internal set; }
        public int TileIndex { get; internal set; }
    }
}