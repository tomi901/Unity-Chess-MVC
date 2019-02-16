using System.Collections.Generic;


namespace Chess
{
    public class PieceQueen : Piece
    {

        protected override IEnumerable<BoardMovement> GetAllPosibleRelativeMovements(Board board)
        {
            foreach (BoardMovement movement in GetHorizontalAndVerticalLines(board))
            {
                yield return movement;
            }
            foreach (BoardMovement movement in GetDiagonalLines(board))
            {
                yield return movement;
            }
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceQueen();
        }

    }

}
