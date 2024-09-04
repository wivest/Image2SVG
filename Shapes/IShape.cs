using SkiaSharp;

namespace Image2SVG.Shapes
{
    interface IShape<T>
    {
        public byte Alpha { get; set; }
        public SKColor Color { get; set; }
        public SKRectI Bounds { get; }

        public void Draw(SKCanvas canvas);
        public void RandomizeParameters(SKImageInfo info);
        public T Mutate(float percentage);
    }
}
