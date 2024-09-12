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
            SKBitmap bitmap = Resize(filename, 0.5f);
            source = SKSurface.Create(bitmap.Info);
            source.Canvas.DrawBitmap(bitmap, 0, 0);

            generated = SKSurface.Create(bitmap.Info);
            generated.Canvas.DrawPaint(new SKPaint { Color = SKColors.White });
            Save(generated, "images/pregeneration.png");
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

        public void SaveGenerated(string filename)
        {
            Save(generated, filename);
        }

        private void Save(SKSurface surface, string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            SKImage generatedImage = surface.Snapshot();
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
                Save(generated, $"images/generation{i}.png");
                stopwatch.Stop();
                Console.WriteLine($"Shape {i + 1}: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}
