using System.Collections.Generic;

namespace Chess
{
    public class PieceKnight : Piece
    {

        public override PieceType Type => PieceType.Knight;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(IBoard board)
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

    }
}