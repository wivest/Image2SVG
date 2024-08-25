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

        public void Generate(IShape shape, int count, int samples)
        {
            generated.Canvas.DrawPaint(backgroundPaint);

            var minScore = int.MaxValue;
            SKSurface newGenerated = SKSurface.Create(image.Info);

            for (int i = 0; i < count; i++)
            {
                SKSurface currentGeneratedCopy = SKSurface.Create(image.Info);

                for (int sample = 0; sample < samples; sample++)
                {
                    currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);
                    shape.RandomizeParameters(image.Info);
                    shape.Draw(currentGeneratedCopy.Canvas);

                    var score = CalculateScore(currentGeneratedCopy);
                    if (score < minScore)
                    {
                        minScore = score;
                        newGenerated = currentGeneratedCopy;
                    }
                }

                generated = newGenerated;
                minScore = int.MaxValue;
            }
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
