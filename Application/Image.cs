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

        public Image(FileInfo fileInfo, float scale)
        {
            SKBitmap bitmap = Resize(fileInfo.FullName, scale);
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

        public void SaveTo(DirectoryInfo directory, string filename)
        {
            SKImage generatedImage = generated.Snapshot();
            using var pngStream = new FileStream(
                $"{directory.FullName}/{filename}.png",
                FileMode.Create
            );
            generatedImage.Encode().SaveTo(pngStream);

            var svg = new XmlDocument();
            XmlElement root = svg.CreateElement("svg");
            root.SetAttribute("width", $"{generator.Info.Width}");
            root.SetAttribute("height", $"{generator.Info.Height}");
            root.SetAttribute("style", "background-color:white");
            svg.AppendChild(root);

            foreach (T shape in shapes)
            {
                root.AppendChild(shape.ToSVG(svg));
            }

            using var stream = new FileStream(
                $"{directory.FullName}/{filename}.svg",
                FileMode.Create
            );
            svg.Save(stream);
        }

        public void Generate(int numberOfShapes, int samples, int mutations, int generations)
        {
            for (int i = 0; i < numberOfShapes; i++)
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                T shape = generator.EvolveShapes(
                    samples,
                    mutations,
                    generations,
                    GetDegreeOfDetail()
                );
                shape.Draw(generated.Canvas);
                shapes.Add(shape);

                stopwatch.Stop();
                ClearLine();
                Console.Write($"Shape {i + 1}: {stopwatch.ElapsedMilliseconds} ms");
            }
            ClearLine();
        }

        private int GetDegreeOfDetail()
        {
            int countSqrt = (int)Math.Pow(shapes.Count, 0.5);
            return Math.Max(1, countSqrt);
        }

        private static void ClearLine()
        {
            int currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, currentLine);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, currentLine);
        }
    }
}
