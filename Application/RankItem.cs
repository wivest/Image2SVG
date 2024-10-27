using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class RankItem : IComparable<RankItem>
    {
        public IShape Shape;
        private long difference;

        private readonly Generator generator;

        public RankItem(IShape shape, Generator generator)
        {
            Shape = shape;
            this.generator = generator;
            Calculate(Shape);
        }

        private void Calculate(IShape shape)
        {
            using SKSurface currentGeneratedCopy = SKSurface.Create(generator.Info);
            currentGeneratedCopy.Canvas.DrawSurface(generator.Generated, 0, 0);
            shape.Draw(currentGeneratedCopy.Canvas);

            long currentDifference = generator.ImageDifference.GetBoundsValue(shape.ImageBounds);
            long newDifference = CalculateDifference(currentGeneratedCopy, shape.ImageBounds);
            difference = newDifference - currentDifference;
        }

        private long CalculateDifference(SKSurface current, SKRectI bounds)
        {
            long difference = 0;

            ReadOnlySpan<byte> originalPixels = generator.Source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = current.PeekPixels().GetPixelSpan();

            int bytesPerRow = generator.Info.RowBytes;
            int bytesPerPixel = generator.Info.BytesPerPixel;

            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                int offset = y * bytesPerRow;
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    int index = offset + x * bytesPerPixel;
                    difference += generator.CalculatePixelDifference(
                        originalPixels,
                        currentPixels,
                        index
                    );
                }
            }

            return difference;
        }

        public int CompareTo(RankItem? other)
        {
            if (other == null)
                return 0;
            if (difference < other.difference)
                return -1;
            if (difference > other.difference)
                return 1;
            return 0;
        }
    }
}
