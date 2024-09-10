using SkiaSharp;

namespace Image2SVG.Shapes
{
    interface IShape<T>
    {
        public byte Alpha { get; set; }
        public SKColor Color { get; set; }

        public SKImageInfo Info { get; }
        public SKRectI Bounds { get; }
        public SKRectI ImageBounds
        {
            get { return new SKRectI(); }
        }

        public void Draw(SKCanvas canvas);
        public void RandomizeParameters(SKImageInfo info);
        public T Mutate(float percentage);
    }
}
