using SkiaSharp;

namespace Image2SVG.Application
{
    class Precalculated<T>
    {
        public T[,] Data;

        public Precalculated(SKImageInfo info)
        {
            Data = new T[info.Height, info.Width];
        }
    }
}
