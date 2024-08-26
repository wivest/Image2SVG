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
            var random = new Random();

            x *= 1 + (float)random.NextDouble() * percentage;
            y *= 1 + (float)random.NextDouble() * percentage;
            width *= 1 + (float)random.NextDouble() * percentage;
            height *= 1 + (float)random.NextDouble() * percentage;

            return new Rect();
        }
    }
}
