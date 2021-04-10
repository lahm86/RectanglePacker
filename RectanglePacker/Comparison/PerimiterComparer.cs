using System.Collections.Generic;

namespace RectanglePacker.Comparison
{
    public class PerimiterComparer<R> : IComparer<R> where R : IRectangle
    {
        public int Compare(R r1, R r2)
        {
            return r1.Perimiter.CompareTo(r2.Perimiter);
        }
    }
}