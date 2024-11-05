using System.Xml;
using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Model
{
    class Image
    {
        private readonly SKSurface source;
        private readonly SKSurface generated;
        private readonly List<IShape> shapes = new();

        private readonly Generator generator;
        private readonly Animator animator;

        public Image(FileInfo fileInfo, float scale)
        {
            using SKBitmap bitmap = SKBitmap.Decode(fileInfo.FullName);
            SKImageInfo size = ScaleSize(bitmap.Info, scale);

            source = SKSurface.Create(size);
            Resize(bitmap, size);
            generated = SKSurface.Create(size);
            generated.Canvas.DrawPaint(new SKPaint { Color = SKColors.White });

            generator = new Generator(size, source, generated);
            animator = new Animator { FrameRate = 24 };
        }

        public void SaveTo(DirectoryInfo directory, string filename)
        {
            var svg = new XmlDocument();
            XmlElement root = svg.CreateElement("svg");
            root.SetAttribute("width", $"{generator.Info.Width}");
            root.SetAttribute("height", $"{generator.Info.Height}");
            root.SetAttribute("style", "background-color:white");
            svg.AppendChild(root);

            foreach (IShape shape in shapes)
            {
                root.AppendChild(shape.ToSVG(svg));
            }

            using var stream = new FileStream(
                $"{directory.FullName}/{filename}.svg",
                FileMode.Create
            );
            svg.Save(stream);

            Console.WriteLine($"Saved to {stream.Name}.");

            if (animator.SaveTo(directory, filename))
                Console.WriteLine("Video saved successfully.");
        }

        public void Generate(int numberOfShapes, int samples, int mutations, int generations)
        {
            for (int i = 0; i < numberOfShapes; i++)
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                IShape shape = generator.EvolveShapes(
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

        private static SKImageInfo ScaleSize(SKImageInfo size, float scale)
        {
            return size.WithSize((int)(size.Width * scale), (int)(size.Height * scale));
        }

        private void Resize(SKBitmap bitmap, SKImageInfo size)
        {
            bitmap = bitmap.Resize(size, SKFilterQuality.High);
            source.Canvas.DrawBitmap(bitmap, 0, 0);
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
