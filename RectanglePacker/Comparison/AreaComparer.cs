using System.Collections.Generic;

namespace RectanglePacker.Comparison
{
    public class AreaComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            return r1.Area.CompareTo(r2.Area);
        }
    }
}