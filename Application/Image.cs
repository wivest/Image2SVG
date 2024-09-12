using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Image<T>
        where T : IShape<T>, new()
    {
        private const string RESIZED_FOLDER = "resized/";

        private readonly SKSurface source;
        private readonly SKSurface generated;
        private readonly Generator<T> generator;

        public Image(string filename)
        {
            SKImage image = SKImage.FromEncodedData(Resize(filename, 0.5f));
            source = SKSurface.Create(image.Info);
            source.Canvas.DrawImage(image, 0, 0);

            generated = SKSurface.Create(image.Info);
            generated.Canvas.DrawPaint(new SKPaint { Color = SKColors.White });
            generator = new Generator<T>(image.Info, source, generated);
        }

        public string Resize(string filename, float scale)
        {
            SKImage source = SKImage.FromEncodedData(filename);
            SKBitmap bitmap = SKBitmap.Decode(filename);
            var size = source.Info.WithSize(
                (int)(source.Width * scale),
                (int)(source.Height * scale)
            );
            bitmap = bitmap.Resize(size, SKFilterQuality.High);

            SKSurface resizedSurface = SKSurface.Create(bitmap.Info);
            resizedSurface.Canvas.DrawBitmap(bitmap, 0, 0);
            string resizedFilename = RESIZED_FOLDER + filename;
            Save(resizedSurface, resizedFilename);
            return resizedFilename;
        }

        public void SaveGenerated(string filename)
        {
            Save(generated, filename);
        }

        private void Save(SKSurface surface, string filename)
        {
            SKImage generatedImage = surface.Snapshot();
            using var stream = new FileStream(filename, FileMode.Create);
            generatedImage.Encode().SaveTo(stream);
        }

        public void Generate(int numberOfShapes)
        {
            for (int i = 0; i < numberOfShapes; i++)
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                T shape = generator.EvolveShapes(50, 5, 5);
                shape.Draw(generated.Canvas);
                stopwatch.Stop();
                Console.WriteLine($"Shape {i + 1}: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
