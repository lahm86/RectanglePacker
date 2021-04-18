namespace RectanglePacker.Organisation
{
    public class PackingOptions
    {
        public PackingFillMode FillMode { get; set; }
        public PackingGroupMode GroupMode { get; set; }
        public PackingOrderMode OrderMode { get; set; }
        public PackingOrder Order { get; set; }
        public PackingStartMethod StartMethod { get; set; }
    }
}