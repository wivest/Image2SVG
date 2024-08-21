using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Image
{
    class Image
    {
        SKImage image;

        SKSurface generated;

        public Image(SKImage image, SKColor backgroundColor)
        {
            this.image = image;

            var paint = new SKPaint { Color = backgroundColor };
            generated = SKSurface.Create(this.image.Info);
            generated.Canvas.DrawPaint(paint);
        }

        public void Randomize(IShape shape, int count)
        {
            var random = new Random();

            for (int i = 0; i < count; i++)
            {
                shape.RandomizeParameters();
                shape.Draw(generated.Canvas);
            }
        }

        public void SaveTo(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            SKImage generatedImage = generated.Snapshot();
            generatedImage.Encode().SaveTo(stream);
        }
    }
}
