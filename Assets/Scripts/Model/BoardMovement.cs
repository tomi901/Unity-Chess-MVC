

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

        public bool IsInsideBox(BoardVector boxLength) => from.IsInsideBox(boxLength) && to.IsInsideBox(boxLength);

        public override string ToString()
        {
            return from.ToStringCoordinates() + to.ToStringCoordinates();
        }

    }
}