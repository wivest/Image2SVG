using SkiaSharp;

namespace Image2SVG.Image
{
    class Image
    {
        SKImage image;

        public Image(SKImage image)
        {
            this.image = image;
        }

        public void SaveTo(string filename)
        {
            FileStream stream = new(filename, FileMode.Create);
            image.Encode().SaveTo(stream);
        }
    }
}
