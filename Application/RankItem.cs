using Image2SVG.Shapes;

namespace Image2SVG.Application
{
    class RankItem<T> : IComparable<RankItem<T>>
        where T : IShape<T>
    {
        public T Shape;
        public long Difference;

        public RankItem(T shape, long difference)
        {
            Shape = shape;
            Difference = difference;
        }

        public int CompareTo(RankItem<T>? other)
        {
            if (other == null)
                return 0;
            if (Difference < other.Difference)
                return -1;
            if (Difference > other.Difference)
                return 1;
            return 0;
        }
    }
}
