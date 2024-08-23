using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Rect : IShape
    {
        private float x;
        private float y;
        private float width;
        private float height;

        private SKPaint paint = new();

        public void Draw(SKCanvas canvas)
        {
            canvas.DrawRect(x, y, width, height, paint);
        }

        public void RandomizeParameters(SKImageInfo info)
        {
            var random = new Random();

            x = (float)random.NextDouble() * info.Width;
            y = (float)random.NextDouble() * info.Height;
            width = (float)random.NextDouble() * info.Width;
            height = (float)random.NextDouble() * info.Height;

            var color = new byte[3];
            random.NextBytes(color);

            paint.Color = new SKColor(color[0], color[1], color[2]);
        }
    }
}
