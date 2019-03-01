using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class PieceKing : Piece
    {

        public const int CastlingDistance = 2;

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

                for (int x = -CastlingDistance; x <= CastlingDistance; x += CastlingDistance * 2)
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

        protected override void OnMovementDone(BoardVector deltaMovement)
        {
            if (MovementsDone >= 50)
            {
                // Tie
            }

            // After castling
            if (Math.Abs(deltaMovement.horizontal) == CastlingDistance)
            {
                Piece castlingRook = ContainingBoard.Raycast(Coordinates, deltaMovement) as PieceRook;
                if (castlingRook == null)
                    throw new Exception("Castling invalid, no rook found.");

                BoardVector rookTargetPos = Coordinates - deltaMovement.Normalized;
                ContainingBoard.DoMovement(new BoardMovement(castlingRook.Coordinates, rookTargetPos));
            }
        }

        protected override Piece InstantiateCopy()
        {
            return new PieceKing();
        }

    }

}