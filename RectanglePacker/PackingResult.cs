using System;
using System.Collections.Generic;

namespace RectanglePacker
{
    public class PackingResult<R> where R : class, IRectangle
    {
        public Packer<R> Packer { get; private set; }
        public double TotalSpaceOccupation { get; private set; }
        public int UsedTileCount => Packer.Tiles.Count;
        public int OrphanCount => Packer.OrphanedRectangles.Count;
        public TimeSpan PackingTime { get; private set; }

        private DateTime _startTime;

        internal PackingResult(Packer<R> packer)
        {
            Packer = packer;
        }

        internal void BeginTimer()
        {
            _startTime = DateTime.Now;
        }

        internal void EndTimer()
        {
            PackingTime = DateTime.Now.Subtract(_startTime);
            int spaceUsed = 0;
            IReadOnlyList<Tile<R>> tiles = Packer.Tiles;
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile<R> tile = tiles[i];
                spaceUsed += tile.UsedSpace;
            }
            TotalSpaceOccupation = Math.Round(100 * (double)spaceUsed / (tiles[0].Area * tiles.Count), 2);
        }
    }
}