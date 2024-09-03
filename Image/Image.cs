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
                T shape = EvolveShapes<T>(samples, 10, 10);
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
                shape.Draw(currentGeneratedCopy.Canvas);

                var difference = CalculateScore<T>(currentGeneratedCopy, shape);
                rank.Add(new Tuple<T, int>(shape, difference));
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
                var shape = new T { Alpha = 128 };
                shape.RandomizeParameters(image.Info);
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

        public int CalculateScore<T>(SKSurface result, IShape<T> shape)
            where T : IShape<T>
        {
            var score = 0;

            ReadOnlySpan<byte> originalPixels = original.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> resultPixels = result.PeekPixels().GetPixelSpan();

            int bytesPerRow = image.Info.RowBytes;
            int bytesPerPixel = image.Info.BytesPerPixel;

            int bottom = Math.Min(image.Height, (int)shape.Bounds.Bottom);
            int right = Math.Min(image.Width, (int)shape.Bounds.Right);

            for (int y = (int)shape.Bounds.Top; y < bottom; y++)
            {
                var offset = y * bytesPerRow;
                for (int x = (int)shape.Bounds.Left; x < right; x++)
                {
                    for (int channel = 0; channel < bytesPerPixel; channel++)
                    {
                        int i = offset + x * bytesPerPixel + channel;
                        score += Math.Abs(originalPixels[i] - resultPixels[i]);
                    }
                }
            }

            return score;
        }

        public void SaveTo(string filename)
        {
            var stream = new FileStream(filename, FileMode.Create);
            SKImage generatedImage = generated.Snapshot();
            generatedImage.Encode().SaveTo(stream);
        }
    }
}
