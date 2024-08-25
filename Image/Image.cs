using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Image
{
    class Image
    {
        SKImage image;
        SKSurface original;
        SKSurface generated;

        SKPaint backgroundPaint;

        public Image(SKImage image, SKColor backgroundColor)
        {
            this.image = image;
            original = SKSurface.Create(this.image.Info);
            original.Canvas.DrawImage(this.image, 0, 0);
            generated = SKSurface.Create(this.image.Info);

            backgroundPaint = new SKPaint { Color = backgroundColor };
        }

        public void Generate<T>(int count, int samples)
            where T : IShape, new()
        {
            generated.Canvas.DrawPaint(backgroundPaint);

            for (int i = 0; i < count; i++)
            {
                List<Tuple<T, int>> shapes = RankShapes<T>(samples);
                shapes[0].Item1.Draw(generated.Canvas);
            }
        }

        public List<Tuple<T, int>> RankShapes<T>(int samples)
            where T : IShape, new()
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(image.Info);
            var shapes = new List<Tuple<T, int>>();

            for (int sample = 0; sample < samples; sample++)
            {
                currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);

                var shape = new T { Alpha = 128 };
                shape.RandomizeParameters(image.Info);
                shape.Draw(currentGeneratedCopy.Canvas);

                var score = CalculateScore(currentGeneratedCopy);
                shapes.Add(new Tuple<T, int>(shape, score));
            }

            shapes.Sort((Tuple<T, int> a, Tuple<T, int> b) => a.Item2.CompareTo(b.Item2));

            return shapes;
        }

        public int CalculateScore(SKSurface result)
        {
            var score = 0;

            ReadOnlySpan<byte> originalPixels = original.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> resultPixels = result.PeekPixels().GetPixelSpan();

            for (int i = 0; i < originalPixels.Length; i++)
            {
                score += Math.Abs(originalPixels[i] - resultPixels[i]);
            }

            return score;
        }

        public void SaveTo(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            SKImage generatedImage = generated.Snapshot();
            generatedImage.Encode().SaveTo(stream);
        }
    }
}
