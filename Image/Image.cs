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

        public void Generate(IShape shape, int count)
        {
            generated.Canvas.DrawPaint(backgroundPaint);

            for (int i = 0; i < count; i++)
            {
                shape.RandomizeParameters(image.Info);
                shape.Draw(generated.Canvas);
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
