using System.Collections.Generic;

namespace Chess
{
    public class PieceRook : Piece
    {

        public override PieceType Type => PieceType.Rook;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(IBoard board)
        {
            return GetHorizontalAndVerticalLines(board);
        }

    }
}
