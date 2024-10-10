using System.Xml;
using SkiaSharp;

namespace Image2SVG.Shapes
{
    class Rect : IShape<Rect>
    {
        private SKSize size = new();

        public SKColor Color { get; set; }
        public SKPoint Center { get; set; }

        public SKImageInfo Info { get; set; }
        public SKRectI Bounds
        {
            get
            {
                return new SKRectI(
                    (int)(Center.X - size.Width / 2),
                    (int)(Center.Y - size.Height / 2),
                    (int)(Center.X + size.Width / 2),
                    (int)(Center.Y + size.Height / 2)
                );
            }
        }

        public void Draw(SKCanvas canvas)
        {
            var paint = new SKPaint { Color = Color };
            canvas.DrawRect(
                Center.X - size.Width / 2,
                Center.Y - size.Height / 2,
                size.Width,
                size.Height,
                paint
            );
        }

        public void RandomizeParameters(SKRect area)
        {
            var random = new Random();

            float x = (float)random.NextDouble() * area.Width;
            float y = (float)random.NextDouble() * area.Height;
            Center = new SKPoint(area.Left + x, area.Top + y);

            size.Width = (float)random.NextDouble() * area.Width;
            size.Height = (float)random.NextDouble() * area.Height;
        }

        public Rect Mutate(float percentage)
        {
            var clone = new Rect
            {
                Center = Center,
                size = size,
                Color = Color,
                Info = Info
            };
            var random = new Random();

            float x = clone.Center.X * (1 + (1 - 2 * (float)random.NextDouble()) * percentage);
            float y = clone.Center.Y * (1 + (1 - 2 * (float)random.NextDouble()) * percentage);
            clone.Center = new SKPoint(x, y);
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

            element.SetAttribute("x", $"{Center.X - size.Width / 2}");
            element.SetAttribute("y", $"{Center.Y - size.Height / 2}");
            element.SetAttribute("width", $"{size.Width}");
            element.SetAttribute("height", $"{size.Height}");
            element.SetAttribute("fill", $"rgb({Color.Red},{Color.Green},{Color.Blue})");
            element.SetAttribute("fill-opacity", $"{Color.Alpha / 255.0f}");

            return element;
        }
    }
}
