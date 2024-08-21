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

        public void RandomizeParameters()
        {
            var random = new Random();

            x = (float)random.NextDouble();
            y = (float)random.NextDouble();
            width = (float)random.NextDouble();
            height = (float)random.NextDouble();

            var color = new byte[3];
            random.NextBytes(color);

            paint.Color = new SKColor(color[0], color[1], color[2]);
        }
    }
}
