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

        public void Randomize(SKDrawable drawable, int count)
        {
            Random random = new();

            for (int i = 0; i < count; i++)
            {
                double xRelative = random.NextDouble();
                double yRelative = random.NextDouble();
                float x = image.Width * (float)xRelative;
                float y = image.Width * (float)yRelative;

                generated.Canvas.DrawDrawable(drawable, x, y);
            }
        }

        public void SaveTo(string filename)
        {
            FileStream stream = new(filename, FileMode.Create);
            SKImage generatedImage = generated.Snapshot();
            generatedImage.Encode().SaveTo(stream);
        }
    }
}
