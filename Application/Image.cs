using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Image
    {
        private readonly SKImage image;
        private readonly SKSurface source;
        private readonly SKSurface generated;
        private readonly Generator generator;

        public Image(string filename)
        {
            image = SKImage.FromEncodedData(filename);
            source = SKSurface.Create(image.Info);
            source.Canvas.DrawImage(image, 0, 0);

            generated = SKSurface.Create(image.Info);
            generated.Canvas.DrawPaint(new SKPaint { Color = SKColors.White });
            generator = new Generator(image.Info, source, generated);
        }

        public void SaveTo(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            SKImage generatedImage = generated.Snapshot();
            generatedImage.Encode().SaveTo(stream);
        }

        public void Generate<T>(int numberOfShapes)
            where T : IShape<T>, new()
        {
            for (int i = 0; i < numberOfShapes; i++)
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                T shape = generator.EvolveShapes<T>(50, 5, 5);
                shape.Draw(generated.Canvas);
                stopwatch.Stop();
                Console.WriteLine($"Shape {i + 1}: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
