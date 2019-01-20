

namespace Chess
{

    public class Tile
    {

        public IBoard Board { get; private set; }
        public BoardVector Coordinates { get; private set; }

        private Piece currentPiece;
        public Piece CurrentPiece
        {
            get => currentPiece;
            set
            {
                if (value == currentPiece) return;

                currentPiece = value;
                currentPiece.CurrentTile = this;
            }
        }

        public bool HasPiece => CurrentPiece != null;


        public Tile(IBoard board, BoardVector coordinates)
        {
            Board = board;
            Coordinates = coordinates;
        }

    }

}