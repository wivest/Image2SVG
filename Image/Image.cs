using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Image
{
    class Image
    {
        SKImage image;

        SKSurface generated;
        SKPaint backgroundPaint;

        public Image(SKImage image, SKColor backgroundColor)
        {
            this.image = image;

            backgroundPaint = new SKPaint { Color = backgroundColor };
            generated = SKSurface.Create(this.image.Info);
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

        public int CalculateScore()
        {
            var score = 0;

            ReadOnlySpan<byte> originalPixels = image.EncodedData.AsSpan();
            ReadOnlySpan<byte> generatedPixels = generated.PeekPixels().GetPixelSpan();

            for (int i = 0; i < originalPixels.Length; i++)
            {
                score += Math.Abs(originalPixels[i] - generatedPixels[i]);
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
