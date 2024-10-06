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
        public readonly AverageColor AverageColor;

        public Generator(SKImageInfo info, SKSurface source, SKSurface generated)
        {
            Info = info;
            Source = source;
            Generated = generated;

            ImageDifference = new Precalculated(info);
            AverageColor = new AverageColor(Info, Source);
        }

        public T EvolveShapes(int samples, int mutations, int generations, int splits)
        {
            PrecalculateDifference();
            SKRectI area = GetWorstArea(splits, splits);

            var shapes = new List<T>();
            for (int i = 0; i < samples * mutations; i++)
            {
                var shape = new T { Info = Info };
                shape.RandomizeParameters(area);
                shape.Color = AverageColor.GetAverageColor(shape.ImageBounds).WithAlpha(128);
                shapes.Add(shape);
            }

            var rank = new Rank<T>();
            rank.RankShapes(this, shapes);
            rank.Ranked.RemoveRange(samples, rank.Ranked.Count - samples);

            for (int generation = 1; generation < generations; generation++)
            {
                shapes = rank.MutateShapes(mutations);
                rank.RankShapes(this, shapes);
                int shapesLeft = Math.Max(1, samples / generation);
                rank.Ranked.RemoveRange(shapesLeft, rank.Ranked.Count - shapesLeft);
            }

            return rank.Ranked[0].Shape;
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
                int pixelDifference = Math.Abs(originalPixels[i] - currentPixels[i]);
                difference += pixelDifference * pixelDifference;
            }

            return difference;
        }

        public void PrecalculateDifference()
        {
            ReadOnlySpan<byte> sourcePixels = Source.PeekPixels().GetPixelSpan();
            ReadOnlySpan<byte> currentPixels = Generated.PeekPixels().GetPixelSpan();

            int bytesPerRow = Info.RowBytes;
            int bytesPerPixel = Info.BytesPerPixel;

            long differenceRowSum = 0;

            for (int col = 0; col < Info.Width; col++)
            {
                int index = col * bytesPerPixel;
                differenceRowSum += CalculatePixelDifference(sourcePixels, currentPixels, index);
                ImageDifference.Data[0, col] = differenceRowSum;
            }

            for (int row = 1; row < Info.Height; row++)
            {
                int rowIndexOffset = row * bytesPerRow;
                differenceRowSum = 0;

                for (int col = 0; col < Info.Width; col++)
                {
                    int index = rowIndexOffset + col * bytesPerPixel;
                    differenceRowSum += CalculatePixelDifference(
                        sourcePixels,
                        currentPixels,
                        index
                    );
                    ImageDifference.Data[row, col] =
                        ImageDifference.Data[row - 1, col] + differenceRowSum;
                }
            }
        }

        public SKRectI GetWorstArea(int horizontal, int vertical)
        {
            var worstArea = new SKRectI(0, 0, Info.Width, Info.Height);
            long worstDifference = long.MinValue;

            for (int h = 0; h < horizontal; h++)
            {
                int left = Info.Width * h / horizontal;
                int right = (Info.Width - 1) * (h + 1) / horizontal;
                for (int v = 0; v < vertical; v++)
                {
                    int top = Info.Height * v / vertical;
                    int bottom = (Info.Height - 1) * (v + 1) / vertical;

                    var rect = new SKRectI(left, top, right, bottom);
                    long difference =
                        ImageDifference.GetBoundsValue(rect) / (rect.Width * rect.Height);
                    if (difference > worstDifference)
                    {
                        worstDifference = difference;
                        worstArea = rect;
                    }
                }
            }

            return worstArea;
        }
    }
}
