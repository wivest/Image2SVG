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
            get
            {
                return new SKRectI(
                    Math.Max(0, Bounds.Left),
                    Math.Max(0, Bounds.Top),
                    Math.Min(Info.Width, Bounds.Right),
                    Math.Min(Info.Height, Bounds.Bottom)
                );
            }
        }

        public void Draw(SKCanvas canvas);
        public void RandomizeParameters(SKImageInfo info);
        public T Mutate(float percentage);
    }
}
