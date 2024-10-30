using SkiaSharp;

namespace Image2SVG.Model
{
    class AverageColor
    {
        private readonly Precalculated redChannel;
        private readonly Precalculated greenChannel;
        private readonly Precalculated blueChannel;

        private readonly SKImageInfo info;
        private readonly SKSurface source;

        public AverageColor(SKImageInfo info, SKSurface source)
        {
            this.info = info;
            this.source = source;

            redChannel = new Precalculated(info);
            greenChannel = new Precalculated(info);
            blueChannel = new Precalculated(info);
            PrecalculateChannel(redChannel, 2);
            PrecalculateChannel(greenChannel, 1);
            PrecalculateChannel(blueChannel, 0);
        }

        public SKColor GetAverageColor(SKRectI rect)
        {
            long redSum = redChannel.GetBoundsValue(rect);
            long greenSum = greenChannel.GetBoundsValue(rect);
            long blueSum = blueChannel.GetBoundsValue(rect);

            int area = Math.Max(1, rect.Width * rect.Height);

            byte r = (byte)(redSum / area);
            byte g = (byte)(greenSum / area);
            byte b = (byte)(blueSum / area);

            return new SKColor(r, g, b);
        }

        private void PrecalculateChannel(Precalculated channel, int channelIndex)
        {
            ReadOnlySpan<byte> sourcePixels = source.PeekPixels().GetPixelSpan();

            long channelRowSum = 0;

            for (int col = 0; col < info.Width; col++)
            {
                int pixelIndex = col * info.BytesPerPixel;
                channelRowSum += sourcePixels[pixelIndex + channelIndex];
                channel.Data[0, col] = channelRowSum;
            }

            for (int row = 1; row < info.Height; row++)
            {
                int rowIndexOffset = row * info.RowBytes;
                channelRowSum = 0;

                for (int col = 0; col < info.Width; col++)
                {
                    int pixelIndex = rowIndexOffset + col * info.BytesPerPixel;
                    channelRowSum += sourcePixels[pixelIndex + channelIndex];
                    channel.Data[row, col] = channel.Data[row - 1, col] + channelRowSum;
                }
            }
        }
    }
}
