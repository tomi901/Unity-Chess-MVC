

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

                if (currentPiece != null)
                {
                    // Make sure we clear the previous tile to make sure it moves correctly
                    currentPiece.CurrentTile?.ClearPiece();
                    currentPiece.CurrentTile = this;
                }
            }
        }

        public bool HasPiece => CurrentPiece != null;


        public Tile(IBoard board, BoardVector coordinates)
        {
            Board = board;
            Coordinates = coordinates;
        }

        private void ClearPiece()
        {
            currentPiece = null;
        }

    }

}