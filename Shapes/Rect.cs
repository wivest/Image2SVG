using System.Xml;
using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Rect : IShape<Rect>
    {
        private SKPoint center = new();
        private SKSize size = new();

        public byte Alpha { get; set; }
        public SKColor Color { get; set; }

        public SKImageInfo Info { get; set; }
        public SKRectI Bounds
        {
            get
            {
                return new SKRectI(
                    (int)(center.X - size.Width / 2),
                    (int)(center.Y - size.Height / 2),
                    (int)(center.X + size.Width / 2),
                    (int)(center.Y + size.Height / 2)
                );
            }
        }

        public void Draw(SKCanvas canvas)
        {
            var paint = new SKPaint { Color = Color };
            canvas.DrawRect(
                center.X - size.Width / 2,
                center.Y - size.Height / 2,
                size.Width,
                size.Height,
                paint
            );
        }

        public void RandomizeParameters(SKImageInfo info)
        {
            var random = new Random();

            center.X = (float)random.NextDouble() * info.Width;
            center.Y = (float)random.NextDouble() * info.Height;

            size.Width = (float)random.NextDouble() * info.Width;
            size.Height = (float)random.NextDouble() * info.Height;
        }

        public Rect Mutate(float percentage)
        {
            var clone = new Rect
            {
                center = center,
                size = size,
                Color = Color,
                Alpha = Alpha,
                Info = Info
            };
            var random = new Random();

            clone.center.X *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.center.Y *= 1 + percentage - 2 * (float)random.NextDouble() * percentage;
            clone.size.Width *= Math.Abs(
                1 + percentage - 2 * (float)random.NextDouble() * percentage
            );
            clone.size.Height *= Math.Abs(
                1 + percentage - 2 * (float)random.NextDouble() * percentage
            );

            return clone;
        }

        public XmlElement ToSVG(XmlDocument root)
        {
            XmlElement element = root.CreateElement("rect");

            element.SetAttribute("x", $"{center.X - size.Width / 2}");
            element.SetAttribute("y", $"{center.Y - size.Height / 2}");
            element.SetAttribute("width", $"{size.Width}");
            element.SetAttribute("height", $"{size.Height}");
            element.SetAttribute("fill", $"rgb({Color.Red},{Color.Green},{Color.Blue})");
            element.SetAttribute("fill-opacity", $"{Color.Alpha / 255.0f}");

            return element;
        }
    }
}
