using SkiaSharp;

namespace Image2SVG.Model
{
    class Precalculated
    {
        public readonly long[,] Data;

        public Precalculated(SKImageInfo info)
        {
            Data = new long[info.Height, info.Width];
        }

        public long GetBoundsValue(SKRectI bounds)
        {
            return Data[bounds.Bottom, bounds.Right]
                - Data[bounds.Top, bounds.Right]
                - Data[bounds.Bottom, bounds.Left]
                + Data[bounds.Top, bounds.Left];
        }
    }
}
