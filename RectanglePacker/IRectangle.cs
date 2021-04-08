namespace RectanglePacker
{
    public interface IRectangle
    {
        int OriginalX { get; }
        int OriginalY { get; }
        int MappedX { get; set; }
        int MappedY { get; set; }
        int Width { get; }
        int Height { get; }
        int Area { get; }
        int Perimiter { get; }
    }
}