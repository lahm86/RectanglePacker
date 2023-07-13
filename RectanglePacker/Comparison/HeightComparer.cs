namespace RectanglePacker.Comparison;

public class HeightComparer<R> : IComparer<R>
    where R : IRectangle
{
    public int Compare(R r1, R r2)
    {
        return r1.Bounds.Height.CompareTo(r2.Bounds.Height);
    }
}
