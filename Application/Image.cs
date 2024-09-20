using System.Xml;
using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Image<T>
        where T : IShape<T>, new()
    {
        private readonly SKSurface source;
        private readonly SKSurface generated;
        private readonly Generator<T> generator;

        private List<T> shapes = new();

        public Image(string filename)
        {
            SKBitmap bitmap = Resize(filename, 0.5f);
            source = SKSurface.Create(bitmap.Info);
            source.Canvas.DrawBitmap(bitmap, 0, 0);

            generated = SKSurface.Create(bitmap.Info);
            generated.Canvas.DrawPaint(new SKPaint { Color = SKColors.White });
            generator = new Generator<T>(bitmap.Info, source, generated);
        }

        public SKBitmap Resize(string filename, float scale)
        {
            SKImage source = SKImage.FromEncodedData(filename);
            SKBitmap bitmap = SKBitmap.Decode(filename);
            var size = source.Info.WithSize(
                (int)(source.Width * scale),
                (int)(source.Height * scale)
            );

            return bitmap.Resize(size, SKFilterQuality.High);
        }

        public void SaveTo(string folder, string filename)
        {
            SKImage generatedImage = generated.Snapshot();
            using var pngStream = new FileStream($"{folder}{filename}.png", FileMode.Create);
            generatedImage.Encode().SaveTo(pngStream);

            var svg = new XmlDocument();
            XmlElement root = svg.CreateElement("svg");
            root.SetAttribute("width", $"{generator.Info.Width}");
            root.SetAttribute("height", $"{generator.Info.Height}");
            svg.AppendChild(root);
            using var stream = new FileStream($"{folder}{filename}.svg", FileMode.Create);
            svg.Save(stream);
        }

        public void Generate(int numberOfShapes)
        {
            for (int i = 0; i < numberOfShapes; i++)
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                T shape = generator.EvolveShapes(50, 5, 5);
                shape.Draw(generated.Canvas);
                shapes.Add(shape);

                stopwatch.Stop();
                Console.WriteLine($"Shape {i + 1}: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
