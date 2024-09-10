using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Circle : IShape<Circle>
    {
        private float x;
        private float y;
        private float radius;

        public byte Alpha { get; set; }
        public SKColor Color { get; set; }

        public SKImageInfo Info { get; }
        public SKRectI Bounds { get; }
        public SKRectI ImageBounds
        {
            get
            {
                return new SKRectI(
                    (int)(x - radius),
                    (int)(y + radius),
                    (int)(x + radius),
                    (int)(y + radius)
                );
            }
        }

        public void Draw(SKCanvas canvas)
        {
            var paint = new SKPaint { Color = Color };
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

            Color = new SKColor(color[0], color[1], color[2], Alpha);
        }

        public Circle Mutate(float percentage)
        {
            var clone = new Circle
            {
                x = x,
                y = y,
                radius = radius,
                Color = Color,
                Alpha = Alpha
            };
            var random = new Random();

            clone.x *= 1 + (float)random.NextDouble() * percentage;
            clone.y *= 1 + (float)random.NextDouble() * percentage;
            clone.radius *= 1 + (float)random.NextDouble() * percentage;
            var r = (byte)(clone.Color.Red * 1 + (float)random.NextDouble() * percentage);
            var g = (byte)(clone.Color.Green * 1 + (float)random.NextDouble() * percentage);
            var b = (byte)(clone.Color.Blue * 1 + (float)random.NextDouble() * percentage);
            clone.Color = new SKColor(r, g, b, Alpha);

            return clone;
        }
    }
}
