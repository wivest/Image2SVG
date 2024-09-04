using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Application
{
    class Generator
    {
        public List<Tuple<T, int>> RankShapes<T>(
            List<T> shapes,
            SKSurface source,
            SKSurface generated,
            SKImageInfo info
        )
            where T : IShape<T>
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(info);
            var rank = new List<Tuple<T, int>>();

            foreach (T shape in shapes)
            {
                currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);
                int currentDifference = CalculateDifference(
                    source,
                    currentGeneratedCopy,
                    shape.Bounds,
                    info
                );

                shape.Draw(currentGeneratedCopy.Canvas);
                int difference = CalculateDifference(
                    source,
                    currentGeneratedCopy,
                    shape.Bounds,
                    info
                );
                rank.Add(new Tuple<T, int>(shape, difference - currentDifference));
            }

            rank.Sort((Tuple<T, int> a, Tuple<T, int> b) => a.Item2.CompareTo(b.Item2));

            return rank;
        }

        public T EvolveShapes<T>(
            int samples,
            int mutations,
            int generations,
            SKSurface source,
            SKSurface generated,
            SKImageInfo info
        )
            where T : IShape<T>, new()
        {
            var shapes = new List<T>();
            for (int i = 0; i < samples * mutations; i++)
            {
                var shape = new T();
                shape.RandomizeParameters(info);
                shape.Color = AverageColor(source, shape.Bounds, info).WithAlpha(128);
                shapes.Add(shape);
            }
            List<Tuple<T, int>> rank = RankShapes<T>(shapes, source, generated, info);

            for (int generation = 1; generation < generations; generation++)
            {
                shapes = MutateShapes<T>(rank, samples, mutations);
                rank = RankShapes<T>(shapes, source, generated, info);
            }

            return rank[0].Item1;
        }

        public List<T> MutateShapes<T>(List<Tuple<T, int>> rank, int samples, int mutations)
            where T : IShape<T>, new()
        {
            var shapes = new List<T>();

            for (int top = 0; top < samples; top++)
            {
                T shape = rank[top].Item1;
                shapes.Add(shape);
                for (int i = 0; i < mutations; i++)
                {
                    shapes.Add(shape.Mutate(0.1f));
                }
            }

            return shapes;
        }

        public int CalculateDifference(
            SKSurface source,
            SKSurface generated,
            SKRectI bounds,
            SKImageInfo info
        )
        {
            var difference = 0;

            ReadOnlySpan<byte> originalPixels = source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> resultPixels = generated.PeekPixels().GetPixelSpan();

            int bytesPerRow = info.RowBytes;
            int bytesPerPixel = info.BytesPerPixel;

            int bottom = Math.Min(info.Height, bounds.Bottom);
            int right = Math.Min(info.Width, bounds.Right);

            for (int y = bounds.Top; y < bottom; y++)
            {
                var offset = y * bytesPerRow;
                for (int x = bounds.Left; x < right; x++)
                {
                    for (int channel = 0; channel < bytesPerPixel; channel++)
                    {
                        int i = offset + x * bytesPerPixel + channel;
                        difference += Math.Abs(originalPixels[i] - resultPixels[i]);
                    }
                }
            }

            return difference;
        }

        public SKColor AverageColor(SKSurface surface, SKRectI bounds, SKImageInfo info)
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
