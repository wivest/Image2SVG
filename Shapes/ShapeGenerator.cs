using SkiaSharp;

namespace Image2SVG.Shapes
{
    class ShapeGenerator<T>
        where T : IShape, new()
    {
        public byte Opacity = 128;
        public float MutationRange = 0.5f;

        private SKImageInfo info;

        public ShapeGenerator(SKImageInfo info)
        {
            this.info = info;
        }

        public T GenerateShape(SKRect area)
        {
            var shape = new T { Info = info };
            shape.RandomizeParameters(area);
            return shape;
        }
    }
}
