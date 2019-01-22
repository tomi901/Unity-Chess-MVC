using System.Collections.Generic;

namespace Chess
{
    public class PieceKing : Piece
    {

        private int currentMoves = 0;

        public override PieceType Type => PieceType.King;

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x != 0 || y != 0) yield return new BoardVector(x, y);
                }
            }
        }

        protected override void OnMovementDone()
        {
            currentMoves++;
            if (currentMoves >= 50)
            {
                // Tie
            }
        }

    }

}