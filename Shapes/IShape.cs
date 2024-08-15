using SkiaSharp;

namespace Image2SVG.Shapes
{
    interface IShape
    {
        public SKDrawable Drawable { get; set; }

        public void RandomizeParameters();
    }
}
