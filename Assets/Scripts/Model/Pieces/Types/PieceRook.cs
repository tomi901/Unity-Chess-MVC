using System.Collections.Generic;

namespace Chess
{
    public class PieceRook : Piece
    {

        protected override IEnumerable<BoardMovement> GetAllPosibleRelativeMovements(Board board)
        {
            return GetHorizontalAndVerticalLines(board);
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceRook();
        }

    }
}
