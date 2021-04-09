using RectanglePacker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectanglePackerWindow.Model
{
    public class RectanglePositionedEventArgs : EventArgs
    {
        public IRectangle Rectangle { get; set; }
        public int TileIndex { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
    }
}