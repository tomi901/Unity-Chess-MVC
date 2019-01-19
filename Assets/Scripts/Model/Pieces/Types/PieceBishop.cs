using System.Collections.Generic;

namespace Chess
{
    public class PieceBishop : Piece
    {

        public override PieceType Type => PieceType.Bishop;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(IBoard board)
        {
            return GetDiagonalLines(board);
        }

    }
}
