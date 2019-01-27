using System.Collections.Generic;

namespace Chess
{
    public class PieceKing : Piece
    {

        public int CurrentMoves { get; private set; } = 0;

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
            CurrentMoves++;
            if (CurrentMoves >= 50)
            {
                // Tie
            }
        }

        public override Piece MakeCopy()
        {
            return new PieceKing() { CurrentMoves = this.CurrentMoves };
        }

    }

}