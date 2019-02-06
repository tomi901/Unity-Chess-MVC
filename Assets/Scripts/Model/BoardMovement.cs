

namespace Chess
{
    public struct BoardMovement
    {

        public BoardVector from, to;

        public BoardMovement(BoardVector from, BoardVector to)
        {
            this.from = from;
            this.to = to;
        }

        public override string ToString()
        {
            return from.ToStringCoordinates() + to.ToStringCoordinates();
        }

    }
}