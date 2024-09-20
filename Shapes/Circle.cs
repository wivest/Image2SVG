using System.Xml;
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

        public SKImageInfo Info { get; set; }
        public SKRectI Bounds
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
        }

        public Circle Mutate(float percentage)
        {
            var clone = new Circle
            {
                x = x,
                y = y,
                radius = radius,
                Color = Color,
                Alpha = Alpha,
                Info = Info
            };
            var random = new Random();

            clone.x *= 1 + (float)random.NextDouble() * percentage;
            clone.y *= 1 + (float)random.NextDouble() * percentage;
            clone.radius *= 1 + (float)random.NextDouble() * percentage;

            return clone;
        }

        public XmlElement ToSVG(XmlDocument root)
        {
            throw new NotImplementedException();
        }
    }
}
