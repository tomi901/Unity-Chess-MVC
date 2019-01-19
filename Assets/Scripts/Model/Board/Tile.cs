

namespace Chess
{

    public class Tile
    {

        public Board Board { get; private set; }

        public BoardVector Position { get; private set; }

        public Piece CurrentPiece { get; private set; }

    }

}