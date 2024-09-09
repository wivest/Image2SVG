using Image2SVG.Shapes;

namespace Image2SVG.Application
{
    class Rank<T> : List<Tuple<T, int>>
        where T : IShape<T>
    {
        public List<T> MutateShapes(int mutations)
        {
            var shapes = new List<T>();

            foreach (var item in this)
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
    }
}
