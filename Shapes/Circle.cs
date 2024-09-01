using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Circle : IShape<Circle>
    {
        private float x;
        private float y;
        private float radius;

        private SKPaint paint = new();

        public byte Alpha { get; set; }
        public SKRect Bounds
        {
            get { return new SKRect(x - radius, y + radius, x + radius, y + radius); }
        }

        public void Draw(SKCanvas canvas)
        {
            canvas.DrawCircle(x, y, radius, paint);
        }

        public void RandomizeParameters(SKImageInfo info)
        {
            var random = new Random();

            x = (float)random.NextDouble() * info.Width;
            y = (float)random.NextDouble() * info.Height;
            radius = (float)random.NextDouble() * Math.Max(info.Width, info.Height);

            var color = new byte[3];
            random.NextBytes(color);

            paint.Color = new SKColor(color[0], color[1], color[2], Alpha);
        }

        public Circle Mutate(float percentage)
        {
            var clone = new Circle
            {
                x = x,
                y = y,
                radius = radius,
                paint = paint,
                Alpha = Alpha
            };
            var random = new Random();

            clone.x *= 1 + (float)random.NextDouble() * percentage;
            clone.y *= 1 + (float)random.NextDouble() * percentage;
            clone.radius *= 1 + (float)random.NextDouble() * percentage;
            var r = (byte)(clone.paint.Color.Red * 1 + (float)random.NextDouble() * percentage);
            var g = (byte)(clone.paint.Color.Green * 1 + (float)random.NextDouble() * percentage);
            var b = (byte)(clone.paint.Color.Blue * 1 + (float)random.NextDouble() * percentage);
            clone.paint.Color = new SKColor(r, g, b, Alpha);

            return clone;
        }
    }
}
