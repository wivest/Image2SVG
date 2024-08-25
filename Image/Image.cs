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
                T shape = EvolveShapes<T>(samples);
                shape.Draw(generated.Canvas);
            }
        }

        public List<Tuple<T, int>> RankShapes<T>(List<T> shapes)
            where T : IShape
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(image.Info);
            var rank = new List<Tuple<T, int>>();

            foreach (T shape in shapes)
            {
                currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);

                shape.RandomizeParameters(image.Info);
                shape.Draw(currentGeneratedCopy.Canvas);

                var score = CalculateScore(currentGeneratedCopy);
                rank.Add(new Tuple<T, int>(shape, score));
            }

            rank.Sort((Tuple<T, int> a, Tuple<T, int> b) => a.Item2.CompareTo(b.Item2));

            return rank;
        }

        public T EvolveShapes<T>(int samples)
            where T : IShape, new()
        {
            var shapes = new List<T>();
            for (int i = 0; i < samples; i++)
            {
                shapes.Add(new T());
            }
            List<Tuple<T, int>> rank = RankShapes<T>(shapes);

            return rank[0].Item1;
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
