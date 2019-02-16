using System.Collections.Generic;

namespace Chess
{
    public class PieceKnight : Piece
    {

        protected override IEnumerable<BoardMovement> GetAllPosibleRelativeMovements(Board board)
        {
            yield return new BoardMovement(1, 2);
            yield return new BoardMovement(2, 1);

            yield return new BoardMovement(2, -1);
            yield return new BoardMovement(1, -2);

            yield return new BoardMovement(-1, -2);
            yield return new BoardMovement(-2, -1);

            yield return new BoardMovement(-2, 1);
            yield return new BoardMovement(-1, 2);
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceKnight();
        }

    }
}