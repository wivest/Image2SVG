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
        public readonly Precalculated RedChannel;
        public readonly Precalculated GreenChannel;
        public readonly Precalculated BlueChannel;

        public Generator(SKImageInfo info, SKSurface source, SKSurface generated)
        {
            Info = info;
            Source = source;
            Generated = generated;

            ImageDifference = new Precalculated(info);
            RedChannel = new Precalculated(info);
            GreenChannel = new Precalculated(info);
            BlueChannel = new Precalculated(info);
            PrecalculateChannel(RedChannel, 0);
            PrecalculateChannel(GreenChannel, 1);
            PrecalculateChannel(BlueChannel, 2);
        }

        public T EvolveShapes(int samples, int mutations, int generations)
        {
            PrecalculateDifference();

            var shapes = new List<T>();
            for (int i = 0; i < samples * mutations; i++)
            {
                var shape = new T { Info = Info };
                shape.RandomizeParameters(Info);
                shape.Color = AverageColor(shape.ImageBounds).WithAlpha(128);
                shapes.Add(shape);
            }

            var rank = new Rank<T>();
            rank.RankShapes(this, shapes);
            rank.RemoveRange(samples, rank.Count - samples);

            for (int generation = 1; generation < generations; generation++)
            {
                shapes = rank.MutateShapes(mutations);
                rank.RankShapes(this, shapes);
                rank.RemoveRange(samples, rank.Count - samples);
            }

            return rank[0].Item1;
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

        public void PrecalculateChannel(Precalculated channel, int channelIndex)
        {
            ReadOnlySpan<byte> sourcePixels = Source.PeekPixels().GetPixelSpan();
            long channelSum = 0;

            for (int row = 0; row < Info.Height; row++)
            {
                int rowIndexOffset = row * Info.RowBytes;
                long channelRowSum = 0;

                for (int col = 0; col < Info.Width; col++)
                {
                    int pixelIndex = rowIndexOffset + col * Info.BytesPerPixel;
                    channelRowSum += sourcePixels[pixelIndex + channelIndex];
                    channel.Data[row, col] = channelSum + channelRowSum;
                }

                channelSum += channelRowSum;
            }
        }

        public SKColor AverageColor(SKRectI bounds)
        {
            long redSum = RedChannel.GetBoundsValue(bounds);
            long greenSum = GreenChannel.GetBoundsValue(bounds);
            long blueSum = BlueChannel.GetBoundsValue(bounds);

            int area = Math.Max(1, bounds.Width * bounds.Height);

            byte r = (byte)(redSum / area);
            byte g = (byte)(greenSum / area);
            byte b = (byte)(blueSum / area);

            return new SKColor(r, g, b);
        }
    }
}
