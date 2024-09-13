using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Rect : IShape<Rect>
    {
        private float x;
        private float y;
        private float width;
        private float height;

        public byte Alpha { get; set; }
        public SKColor Color { get; set; }

        public SKImageInfo Info { get; set; }
        public SKRectI Bounds
        {
            get { return new SKRectI((int)x, (int)y, (int)(x + width), (int)(y + height)); }
        }

        public void Draw(SKCanvas canvas)
        {
            var paint = new SKPaint { Color = Color };
            canvas.DrawRect(x, y, width, height, paint);
        }

        public void RandomizeParameters(SKImageInfo info)
        {
            var random = new Random();

            var x1 = (float)random.NextDouble() * info.Width;
            var y1 = (float)random.NextDouble() * info.Height;
            var x2 = (float)random.NextDouble() * info.Width;
            var y2 = (float)random.NextDouble() * info.Height;

            x = Math.Min(x1, x2);
            y = Math.Min(y1, y2);
            width = Math.Max(x1, x2) - x;
            height = Math.Max(y1, y2) - y;
        }

        public Rect Mutate(float percentage)
        {
            var clone = new Rect
            {
                x = x,
                y = y,
                width = width,
                height = height,
                Color = Color,
                Alpha = Alpha,
                Info = Info
            };
            var random = new Random();

            clone.x *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.y *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.width *= Math.Abs(1 + percentage - 2 * (float)random.NextDouble() * percentage);
            clone.height *= Math.Abs(1 + percentage - 2 * (float)random.NextDouble() * percentage);

            return clone;
        }
    }
}
