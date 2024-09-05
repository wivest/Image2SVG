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

        public Rank<T> RankShapes<T>(List<T> shapes)
            where T : IShape<T>
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(info);
            var rank = new Rank<T>();

            foreach (T shape in shapes)
            {
                currentGeneratedCopy.Canvas.DrawSurface(generated, 0, 0);
                int currentDifference = CalculateDifference(currentGeneratedCopy, shape.Bounds);

                shape.Draw(currentGeneratedCopy.Canvas);
                int difference = CalculateDifference(currentGeneratedCopy, shape.Bounds);
                rank.Add(new Tuple<T, int>(shape, difference - currentDifference));
            }

            rank.Sort((Tuple<T, int> a, Tuple<T, int> b) => a.Item2.CompareTo(b.Item2));

            return rank;
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
            Rank<T> rank = RankShapes<T>(shapes);

            for (int generation = 1; generation < generations; generation++)
            {
                shapes = MutateShapes<T>(rank, samples, mutations);
                rank = RankShapes<T>(shapes);
            }

            return rank[0].Item1;
        }

        public List<T> MutateShapes<T>(Rank<T> rank, int samples, int mutations)
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
                var offset = y * bytesPerRow;
                for (int x = bounds.Left; x < right; x++)
                {
                    for (int channel = 0; channel < bytesPerPixel; channel++)
                    {
                        int i = offset + x * bytesPerPixel + channel;
                        difference += Math.Abs(originalPixels[i] - currentPixels[i]);
                    }
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
