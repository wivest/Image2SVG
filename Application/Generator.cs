using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Generator
    {
        private SKImageInfo info;
        private SKSurface source;
        private SKSurface generated;

        public Generator(SKImageInfo info, SKSurface source, SKSurface generated)
        {
            this.info = info;
            this.source = source;
            this.generated = generated;
        }

        public void RankShapes<T>(Rank<T> rank, List<T> shapes)
            where T : IShape<T>
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(info);

            foreach (T shape in shapes)
            {
                currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);
                int currentDifference = CalculateDifference(currentGeneratedCopy, shape.Bounds);

                shape.Draw(currentGeneratedCopy.Canvas);
                int difference = CalculateDifference(currentGeneratedCopy, shape.Bounds);
                rank.Add(new Tuple<T, int>(shape, difference - currentDifference));
            }

            rank.Sort((Tuple<T, int> a, Tuple<T, int> b) => a.Item2.CompareTo(b.Item2));
        }

        public T EvolveShapes<T>(int samples, int mutations, int generations)
            where T : IShape<T>, new()
        {
            var shapes = new List<T>();
            for (int i = 0; i < samples * mutations; i++)
            {
                var shape = new T();
                shape.RandomizeParameters(info);
                shape.Color = AverageColor(source, shape.Bounds).WithAlpha(128);
                shapes.Add(shape);
            }

            var rank = new Rank<T>();
            RankShapes<T>(rank, shapes);
            for (int generation = 1; generation < generations; generation++)
            {
                shapes = MutateShapes<T>(rank, mutations);
                RankShapes<T>(rank, shapes);
                rank.RemoveRange(samples, rank.Count - samples);
                Console.WriteLine(generation);
            }

            return rank[0].Item1;
        }

        public List<T> MutateShapes<T>(Rank<T> rank, int mutations)
            where T : IShape<T>
        {
            var shapes = new List<T>();

            foreach (var item in rank)
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

        public int CalculateDifference(SKSurface current, SKRectI bounds)
        {
            var difference = 0;

            ReadOnlySpan<byte> originalPixels = source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = current.PeekPixels().GetPixelSpan();

            int bytesPerRow = info.RowBytes;
            int bytesPerPixel = info.BytesPerPixel;

            int bottom = Math.Min(info.Height, bounds.Bottom);
            int right = Math.Min(info.Width, bounds.Right);

            for (int y = bounds.Top; y < bottom; y++)
            {
                int offset = y * bytesPerRow;
                for (int x = bounds.Left; x < right; x++)
                {
                    int index = offset + x * bytesPerPixel;
                    difference += CalculatePixelDifference(originalPixels, currentPixels, index);
                }
            }

            return difference;
        }

        public SKColor AverageColor(SKSurface surface, SKRectI bounds)
        {
            long r = 0;
            long g = 0;
            long b = 0;

            ReadOnlySpan<byte> pixels = surface.PeekPixels().GetPixelSpan();

            int bytesPerRow = info.RowBytes;
            int bytesPerPixel = info.BytesPerPixel;

            int bottom = Math.Min(info.Height, bounds.Bottom);
            int right = Math.Min(info.Width, bounds.Right);

            for (int y = bounds.Top; y < bottom; y++)
            {
                var offset = y * bytesPerRow;
                for (int x = bounds.Left; x < right; x++)
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
