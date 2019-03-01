using System.Collections.Generic;
using System.Linq;

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
                var nextMovements = new HashSet<BoardVector>(CurrentTurn.NextCalculatedTurnsMovements
                    .Select(kvp => kvp.Key.to));

                for (int x = -2; x <= 2; x += 4)
                {
                    BoardVector movement = new BoardVector(x, 0);

                    BoardVector nextTilePos = Coordinates + movement.Normalized;
                    if (nextMovements.Contains(nextTilePos))
                        continue;

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