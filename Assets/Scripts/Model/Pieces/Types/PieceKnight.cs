using System.Collections.Generic;

namespace Chess
{
    public class PieceKnight : Piece
    {

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            yield return new BoardVector(1, 2);
            yield return new BoardVector(2, 1);

            yield return new BoardVector(2, -1);
            yield return new BoardVector(1, -2);

            yield return new BoardVector(-1, -2);
            yield return new BoardVector(-2, -1);

            yield return new BoardVector(-2, 1);
            yield return new BoardVector(-1, 2);
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceKnight();
        }

    }
}