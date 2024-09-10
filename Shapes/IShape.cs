using SkiaSharp;

namespace Image2SVG.Shapes
{
    interface IShape<T>
    {
        public byte Alpha { get; set; }
        public SKColor Color { get; set; }

        public SKImageInfo Info { get; set; }
        public SKRectI Bounds { get; }
        public SKRectI ImageBounds
        {
            get
            {
                return new SKRectI(
                    Math.Max(0, Math.Min(Info.Width - 1, Bounds.Left)),
                    Math.Max(0, Math.Min(Info.Height - 1, Bounds.Top)),
                    Math.Min(Info.Width - 1, Math.Max(0, Bounds.Right)),
                    Math.Min(Info.Height - 1, Math.Max(0, Bounds.Bottom))
                );
            }
        }

        public void Draw(SKCanvas canvas);
        public void RandomizeParameters(SKImageInfo info);
        public T Mutate(float percentage);
    }
}
