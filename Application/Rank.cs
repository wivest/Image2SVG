using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Rank
    {
        public List<RankItem> Ranked = new();

        private Generator generator;

        public Rank(Generator generator)
        {
            this.generator = generator;
        }

        public void RankShapes(List<IShape> shapes)
        {
            Ranked.EnsureCapacity(Ranked.Count + shapes.Count);

            Parallel.ForEach(
                shapes,
                shape =>
                {
                    long currentDifference = generator.ImageDifference.GetBoundsValue(
                        shape.ImageBounds
                    );

                    using SKSurface currentGeneratedCopy = SKSurface.Create(generator.Info);
                    currentGeneratedCopy.Canvas.DrawSurface(generator.Generated, 0, 0);
                    shape.Draw(currentGeneratedCopy.Canvas);
                    long difference = CalculateDifference(currentGeneratedCopy, shape.ImageBounds);
                    var item = new RankItem(shape, difference - currentDifference);
                    Ranked.Add(item);
                }
            );

            Ranked.Sort();
        }

        public List<IShape> MutateShapes(int mutations)
        {
            var shapes = new List<IShape>();

            foreach (var item in Ranked)
            {
                IShape shape = item.Shape;
                shapes.Add(shape);
                for (int i = 0; i < mutations; i++)
                {
                    IShape mutatedShape = shape.Mutate(0.5f);
                    mutatedShape.Color = generator
                        .BaseColor.GetAverageColor(mutatedShape.ImageBounds)
                        .WithAlpha(generator.Shapes.Opacity);
                    shapes.Add(mutatedShape);
                }
            }

            return shapes;
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
    }
}
