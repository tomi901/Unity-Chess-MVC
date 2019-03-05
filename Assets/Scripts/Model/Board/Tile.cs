

namespace Chess
{

    public class Tile
    {

        public Board Board { get; private set; }
        public BoardVector Coordinates { get; private set; }

        private Piece currentPiece;
        public Piece CurrentPiece
        {
            get => currentPiece;
            set
            {
                if (value == currentPiece) return;

                if (value != null)
                {
                    // Make sure we clear the previous tile to make sure it moves correctly
                    value.CurrentTile?.ClearPiece();
                    value.CurrentTile = this;
                }

                currentPiece = value;
            }
        }

        public bool HasPiece => CurrentPiece != null;


        public Tile(Board board, BoardVector coordinates)
        {
            Board = board;
            Coordinates = coordinates;
        }

        public void ClearPiece()
        {
            currentPiece = null;
        }

    }

}