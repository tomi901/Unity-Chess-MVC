using System.Collections.Generic;

namespace Chess
{
    public class PieceBishop : Piece
    {

        public override PieceType Type => PieceType.Bishop;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            return GetDiagonalLines(board);
        }

        public override Piece MakeCopy()
        {
            return new PieceBishop();
        }

    }
}
