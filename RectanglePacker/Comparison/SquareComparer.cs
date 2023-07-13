namespace RectanglePacker.Comparison;

public class SquareComparer<R> : IComparer<R>
    where R : IRectangle
{
    public int Compare(R r1, R r2)
    {
        bool b1 = r1.Bounds.Width == r1.Bounds.Height;
        bool b2 = r2.Bounds.Width == r2.Bounds.Height;
        if (b1 && b2)
        {
            return r1.Area.CompareTo(r2.Area);
        }

        return b1 ? -1 : 1;
    }
}
