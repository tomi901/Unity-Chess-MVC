using System.Collections.Generic;

namespace Chess
{
    public class PieceBishop : Piece
    {

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            return GetDiagonalLines(board);
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceBishop();
        }

    }
}
