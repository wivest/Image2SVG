using System.Xml;
using SkiaSharp;

namespace Image2SVG.Shapes.Concrete
{
    class Rect : IShape
    {
        private SKPoint center = new();
        private SKSize size = new();

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

        public void RandomizeParameters(SKRect area)
        {
            var random = new Random();

            center.X = area.Left + (float)random.NextDouble() * area.Width;
            center.Y = area.Top + (float)random.NextDouble() * area.Height;

            size.Width = (float)random.NextDouble() * area.Width;
            size.Height = (float)random.NextDouble() * area.Height;
        }

        public IShape Mutate(float percentage)
        {
            var clone = new Rect
            {
                center = center,
                size = size,
                Color = Color,
                Info = Info
            };
            var random = new Random();

            float x = clone.center.X * (1 + (1 - 2 * (float)random.NextDouble()) * percentage);
            float y = clone.center.Y * (1 + (1 - 2 * (float)random.NextDouble()) * percentage);
            clone.center = new SKPoint(x, y);
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
