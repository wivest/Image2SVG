using SkiaSharp;

namespace Image2SVG.Shapes
{
    interface IShape
    {
        public byte Alpha { get; set; }

        public void Draw(SKCanvas canvas);
        public void RandomizeParameters(SKImageInfo info);
        public void TweakParameters(float percentage);
    }
}
