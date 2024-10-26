using Image2SVG.Shapes;

namespace Image2SVG.Application
{
    class RankItem : IComparable<RankItem>
    {
        public IShape Shape;
        public long Difference;

        public RankItem(IShape shape, long difference)
        {
            Shape = shape;
            Difference = difference;
        }

        public int CompareTo(RankItem? other)
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
