using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Generator<T>
        where T : IShape<T>, new()
    {
        public readonly SKImageInfo Info;
        public readonly SKSurface Source;
        public readonly SKSurface Generated;

        public readonly Precalculated ImageDifference;

        public Generator(SKImageInfo info, SKSurface source, SKSurface generated)
        {
            Info = info;
            Source = source;
            Generated = generated;

            ImageDifference = new Precalculated(info);
        }

        public T EvolveShapes(int samples, int mutations, int generations)
        {
            PrecalculateDifference();

            var shapes = new List<T>();
            for (int i = 0; i < samples * mutations; i++)
            {
                var shape = new T { Info = Info };
                shape.RandomizeParameters(Info);
                shape.Color = AverageColor(Source, shape.ImageBounds).WithAlpha(128);
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
            SKSurface currentGeneratedCopy = SKSurface.Create(Info);

            foreach (T shape in shapes)
            {
                long currentDifference = ImageDifference.GetBoundsValue(shape.ImageBounds);

                currentGeneratedCopy.Canvas.DrawSurface(Generated, 0, 0);
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

            for (int channel = 0; channel < Info.BytesPerPixel; channel++)
            {
                int i = pixelIndex + channel;
                difference += Math.Abs(originalPixels[i] - currentPixels[i]);
            }

            return difference;
        }

        public long CalculateDifference(SKSurface current, SKRectI bounds)
        {
            long difference = 0;

            ReadOnlySpan<byte> originalPixels = Source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = current.PeekPixels().GetPixelSpan();

            int bytesPerRow = Info.RowBytes;
            int bytesPerPixel = Info.BytesPerPixel;

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
            ReadOnlySpan<byte> originalPixels = Source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = Generated.PeekPixels().GetPixelSpan();

            int bytesPerRow = Info.RowBytes;
            int bytesPerPixel = Info.BytesPerPixel;

            ImageDifference.Data[0, 0] = CalculatePixelDifference(originalPixels, currentPixels, 0);

            for (int col = 1; col < Info.Width; col++)
            {
                int index = col * bytesPerPixel;
                int difference = CalculatePixelDifference(originalPixels, currentPixels, index);
                ImageDifference.Data[0, col] = difference + ImageDifference.Data[0, col - 1];
            }

            for (int row = 1; row < Info.Height; row++)
            {
                int offset = row * bytesPerRow;

                ImageDifference.Data[row, 0] =
                    CalculatePixelDifference(originalPixels, currentPixels, offset)
                    + ImageDifference.Data[row - 1, 0];
                for (int col = 1; col < Info.Width; col++)
                {
                    int index = offset + col * bytesPerPixel;
                    int difference = CalculatePixelDifference(originalPixels, currentPixels, index);
                    ImageDifference.Data[row, col] =
                        difference
                        + ImageDifference.Data[row - 1, col]
                        + ImageDifference.Data[row, col - 1]
                        - ImageDifference.Data[row - 1, col - 1];
                }
            }
        }

        public SKColor AverageColor(SKSurface surface, SKRectI bounds)
        {
            long r = 0;
            long g = 0;
            long b = 0;

            ReadOnlySpan<byte> pixels = surface.PeekPixels().GetPixelSpan();

            int bytesPerRow = Info.RowBytes;
            int bytesPerPixel = Info.BytesPerPixel;

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
