using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Rank<T> : List<Tuple<T, long>>
        where T : IShape<T>, new()
    {
        public void RankShapes(Generator<T> generator, List<T> shapes)
        {
            Parallel.ForEach(
                shapes,
                shape =>
                {
                    long currentDifference = generator.ImageDifference.GetBoundsValue(
                        shape.ImageBounds
                    );

                    SKSurface currentGeneratedCopy = SKSurface.Create(generator.Info);
                    currentGeneratedCopy.Canvas.DrawSurface(generator.Generated, 0, 0);
                    shape.Draw(currentGeneratedCopy.Canvas);
                    long difference = CalculateDifference(
                        generator,
                        currentGeneratedCopy,
                        shape.ImageBounds
                    );
                    Add(new Tuple<T, long>(shape, difference - currentDifference));
                }
            );

            Sort((Tuple<T, long> a, Tuple<T, long> b) => a.Item2.CompareTo(b.Item2));
        }

        public List<T> MutateShapes(int mutations)
        {
            var shapes = new List<T>();

            foreach (var item in this)
            {
                T shape = item.Item1;
                shapes.Add(shape);
                for (int i = 0; i < mutations; i++)
                {
                    shapes.Add(shape.Mutate(0.1f));
                }
            }

            return shapes;
        }

        private long CalculateDifference(Generator<T> generator, SKSurface current, SKRectI bounds)
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
    }
}
