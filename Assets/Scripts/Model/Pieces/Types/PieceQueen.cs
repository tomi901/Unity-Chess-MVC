using System.Collections.Generic;


namespace Chess
{
    public class PieceQueen : Piece
    {

        public override PieceType Type => PieceType.Queen;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            foreach (BoardVector movement in GetHorizontalAndVerticalLines(board))
            {
                yield return movement;
            }
            foreach (BoardVector movement in GetDiagonalLines(board))
            {
                yield return movement;
            }
        }

    }

}
