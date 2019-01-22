using System.Collections.Generic;

namespace Chess
{
    public class PieceRook : Piece
    {

        public override PieceType Type => PieceType.Rook;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            return GetHorizontalAndVerticalLines(board);
        }

    }
}
