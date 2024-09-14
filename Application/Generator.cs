using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Generator<T>
        where T : IShape<T>, new()
    {
        private SKImageInfo info;
        private SKSurface source;
        private SKSurface generated;

        private Precalculated imageDifference;

        public Generator(SKImageInfo info, SKSurface source, SKSurface generated)
        {
            this.info = info;
            this.source = source;
            this.generated = generated;

            imageDifference = new Precalculated(info);
        }

        public T EvolveShapes(int samples, int mutations, int generations)
        {
            PrecalculateDifference();

            var shapes = new List<T>();
            for (int i = 0; i < samples * mutations; i++)
            {
                var shape = new T { Info = info };
                shape.RandomizeParameters(info);
                shape.Color = AverageColor(source, shape.ImageBounds).WithAlpha(128);
                shapes.Add(shape);
            }

            var rank = new Rank<T>();
            RankShapes(rank, shapes);
            rank.RemoveRange(samples, rank.Count - samples);

            for (int generation = 1; generation < generations; generation++)
            {
                shapes = rank.MutateShapes(mutations);
                RankShapes(rank, shapes);
                rank.RemoveRange(samples, rank.Count - samples);
            }

            return rank[0].Item1;
        }

        public void RankShapes(Rank<T> rank, List<T> shapes)
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(info);

            foreach (T shape in shapes)
            {
                long currentDifference = imageDifference.GetBoundsValue(shape.ImageBounds);

                currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);
                shape.Draw(currentGeneratedCopy.Canvas);
                long difference = CalculateDifference(currentGeneratedCopy, shape.ImageBounds);
                rank.Add(new Tuple<T, int>(shape, (int)(difference - currentDifference)));
            }

            rank.Sort((Tuple<T, int> a, Tuple<T, int> b) => a.Item2.CompareTo(b.Item2));
        }

        public int CalculatePixelDifference(
            ReadOnlySpan<byte> originalPixels,
            ReadOnlySpan<byte> currentPixels,
            int pixelIndex
        )
        {
            int difference = 0;

            for (int channel = 0; channel < info.BytesPerPixel; channel++)
            {
                int i = pixelIndex + channel;
                difference += Math.Abs(originalPixels[i] - currentPixels[i]);
            }

            return difference;
        }

        public long CalculateDifference(SKSurface current, SKRectI bounds)
        {
            long difference = 0;

            ReadOnlySpan<byte> originalPixels = source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = current.PeekPixels().GetPixelSpan();

            int bytesPerRow = info.RowBytes;
            int bytesPerPixel = info.BytesPerPixel;

            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                int offset = y * bytesPerRow;
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    int index = offset + x * bytesPerPixel;
                    difference += CalculatePixelDifference(originalPixels, currentPixels, index);
                }
            }

            return difference;
        }

        public void PrecalculateDifference()
        {
            ReadOnlySpan<byte> originalPixels = source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = generated.PeekPixels().GetPixelSpan();

            int bytesPerRow = info.RowBytes;
            int bytesPerPixel = info.BytesPerPixel;

            imageDifference.Data[0, 0] = CalculatePixelDifference(originalPixels, currentPixels, 0);

            for (int col = 1; col < info.Width; col++)
            {
                int index = col * bytesPerPixel;
                int difference = CalculatePixelDifference(originalPixels, currentPixels, index);
                imageDifference.Data[0, col] = difference + imageDifference.Data[0, col - 1];
            }

            for (int row = 1; row < info.Height; row++)
            {
                int offset = row * bytesPerRow;

                imageDifference.Data[row, 0] =
                    CalculatePixelDifference(originalPixels, currentPixels, offset)
                    + imageDifference.Data[row - 1, 0];
                for (int col = 1; col < info.Width; col++)
                {
                    int index = offset + col * bytesPerPixel;
                    int difference = CalculatePixelDifference(originalPixels, currentPixels, index);
                    imageDifference.Data[row, col] =
                        difference
                        + imageDifference.Data[row - 1, col]
                        + imageDifference.Data[row, col - 1]
                        - imageDifference.Data[row - 1, col - 1];
                }
            }
        }

        public SKColor AverageColor(SKSurface surface, SKRectI bounds)
        {
            long r = 0;
            long g = 0;
            long b = 0;

            ReadOnlySpan<byte> pixels = surface.PeekPixels().GetPixelSpan();

            int bytesPerRow = info.RowBytes;
            int bytesPerPixel = info.BytesPerPixel;

            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                var offset = y * bytesPerRow;
                for (int x = bounds.Left; x < bounds.Right; x++)
                {
                    int pixel = offset + x * bytesPerPixel;
                    r += pixels[pixel];
                    g += pixels[pixel + 1];
                    b += pixels[pixel + 2];
                }
            }

            int size = Math.Max(bounds.Width * bounds.Height, 1);
            r /= size;
            g /= size;
            b /= size;

            return new SKColor((byte)r, (byte)g, (byte)b);
        }
    }
}
