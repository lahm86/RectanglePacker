using RectanglePacker.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RectanglePacker
{
    public interface ITile<R> where R : class, IRectangle
    {
        int Index { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        int Area { get; }
        int FreeSpace { get; }
        int UsedSpace { get; }

        PackingFillMode FillMode { get; set; }

        bool Add(R rectangle);
        bool Remove(R rectangle);
    }
}