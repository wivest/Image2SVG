using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Rect : IShape<Rect>
    {
        private float x;
        private float y;
        private float width;
        private float height;

        private SKPaint paint = new();

        public byte Alpha { get; set; }
        public SKRect Bounds
        {
            get { return new SKRect(x, y, x + width, y + height); }
        }

        public void Draw(SKCanvas canvas)
        {
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

            var color = new byte[3];
            random.NextBytes(color);

            paint.Color = new SKColor(color[0], color[1], color[2], Alpha);
        }

        public Rect Mutate(float percentage)
        {
            var clone = new Rect
            {
                x = x,
                y = y,
                width = width,
                height = height,
                paint = paint,
                Alpha = Alpha
            };
            var random = new Random();

            clone.x *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.y *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.width *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.height *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            var r = (byte)(clone.paint.Color.Red * 1 + (float)random.NextDouble() * percentage);
            var g = (byte)(clone.paint.Color.Green * 1 + (float)random.NextDouble() * percentage);
            var b = (byte)(clone.paint.Color.Blue * 1 + (float)random.NextDouble() * percentage);
            clone.paint.Color = new SKColor(r, g, b, Alpha);

            return clone;
        }
    }
}
