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

        public override Piece MakeCopy()
        {
            return new PieceKnight();
        }

    }
}