using System.Collections.Generic;

namespace Chess
{
    public class PieceKing : Piece
    {

        protected override IEnumerable<BoardMovement> GetAllPosibleRelativeMovements(Board board)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x != 0 || y != 0) yield return new BoardMovement(x, y);
                }
            }
        }

        protected override IEnumerable<BoardMovement> GetAllExtraRelativeMovements(Board board)
        {
            // Castling
            if (Rank == 1 && !HasMoved)
            {
                for (int x = -2; x <= 2; x += 4)
                {
                    BoardVector movement = new BoardVector(x, 0);
                    Piece foundPiece = board.Raycast(Coordinates, movement);
                    if (foundPiece is PieceRook && foundPiece.Rank == 1 && !foundPiece.HasMoved)
                        yield return new BoardMovement(movement);
                }
            }
        }


        protected override void OnMovementDone()
        {
            if (MovementsDone >= 50)
            {
                // Tie
            }
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceKing();
        }

    }

}