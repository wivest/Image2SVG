using Image2SVG.Shapes;
using SkiaSharp;

namespace Image2SVG.Image
{
    class Image
    {
        SKImage image;
        SKSurface original;
        SKSurface generated;

        SKPaint backgroundPaint;

        public Image(SKImage image, SKColor backgroundColor)
        {
            this.image = image;
            original = SKSurface.Create(this.image.Info);
            original.Canvas.DrawImage(this.image, 0, 0);
            generated = SKSurface.Create(this.image.Info);

            backgroundPaint = new SKPaint { Color = backgroundColor };
        }

        public void Generate<T>(int count, int samples)
            where T : IShape<T>, new()
        {
            generated.Canvas.DrawPaint(backgroundPaint);

            for (int i = 0; i < count; i++)
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                T shape = EvolveShapes<T>(samples, 3, 3);
                shape.Draw(generated.Canvas);
                stopwatch.Stop();
                Console.WriteLine($"Shape {i + 1}: {stopwatch.ElapsedMilliseconds} ms");
            }
        }

        public List<Tuple<T, int>> RankShapes<T>(List<T> shapes)
            where T : IShape<T>
        {
            SKSurface currentGeneratedCopy = SKSurface.Create(image.Info);
            var rank = new List<Tuple<T, int>>();

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
                shape.RandomizeParameters(image.Info);
                shape.Color = AverageColor(original, shape.Bounds).WithAlpha(128);
                shapes.Add(shape);
            }
            List<Tuple<T, int>> rank = RankShapes<T>(shapes);

            for (int generation = 1; generation < generations; generation++)
            {
                shapes = MutateShapes<T>(rank, samples, mutations);
                rank = RankShapes<T>(shapes);
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

        public int CalculateDifference(SKSurface result, SKRectI bounds)
        {
            var difference = 0;

            ReadOnlySpan<byte> originalPixels = original.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> resultPixels = result.PeekPixels().GetPixelSpan();

            int bytesPerRow = image.Info.RowBytes;
            int bytesPerPixel = image.Info.BytesPerPixel;

            int bottom = Math.Min(image.Height, bounds.Bottom);
            int right = Math.Min(image.Width, bounds.Right);

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

        public SKColor AverageColor(SKSurface surface, SKRectI bounds)
        {
            long r = 0;
            long g = 0;
            long b = 0;

            ReadOnlySpan<byte> pixels = surface.PeekPixels().GetPixelSpan();

            int bytesPerRow = image.Info.RowBytes;
            int bytesPerPixel = image.Info.BytesPerPixel;

            int bottom = Math.Min(image.Height, bounds.Bottom);
            int right = Math.Min(image.Width, bounds.Right);

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

        public void SaveTo(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            SKImage generatedImage = generated.Snapshot();
            generatedImage.Encode().SaveTo(stream);
        }
    }
}
