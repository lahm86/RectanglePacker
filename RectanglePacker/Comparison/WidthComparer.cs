namespace RectanglePacker.Comparison;

public class WidthComparer<R> : IComparer<R>
    where R : IRectangle
{
    public int Compare(R r1, R r2)
    {
        return r1.Bounds.Width.CompareTo(r2.Bounds.Width);
    }
}
