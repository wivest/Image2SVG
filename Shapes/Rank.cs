using Image2SVG.Application;

namespace Image2SVG.Shapes
{
    class Rank
    {
        public List<RankItem> Ranked = new();

        private Generator generator;

        public Rank(Generator generator)
        {
            this.generator = generator;
        }

        public void RankShapes(List<IShape> shapes)
        {
            Ranked.EnsureCapacity(Ranked.Count + shapes.Count);

            Parallel.ForEach(
                shapes,
                shape =>
                {
                    var item = new RankItem(shape, generator);
                    Ranked.Add(item);
                }
            );

            Ranked.Sort();
        }

        public List<IShape> MutateShapes(int mutations)
        {
            var shapes = new List<IShape>();

            foreach (var item in Ranked)
            {
                IShape shape = item.Shape;
                shapes.Add(shape);
                for (int i = 0; i < mutations; i++)
                {
                    IShape mutatedShape = shape.Mutate(generator.Rects.MutationRange);
                    mutatedShape.Color = generator
                        .BaseColor.GetAverageColor(mutatedShape.ImageBounds)
                        .WithAlpha(generator.Rects.Opacity);
                    shapes.Add(mutatedShape);
                }
            }

            return shapes;
        }
    }
}
