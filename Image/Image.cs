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

        public void SaveTo(string filename)
        {
            FileStream stream = new(filename, FileMode.Create);
            image.Encode().SaveTo(stream);
        }
    }
}
