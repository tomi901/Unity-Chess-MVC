using System.Collections.Generic;

namespace Chess
{
    public class PiecePawn : Piece
    {

        public const int PromoteRank = 8;

        protected readonly ChessPieceType[] promotablePieces =
        {
            ChessPieceType.Rook, ChessPieceType.Knight, ChessPieceType.Bishop, ChessPieceType.Queen
        };


        protected override IEnumerable<BoardMovement> GetAllPosibleRelativeMovements(Board board)
        {
            IEnumerable<BoardMovement> All()
            {
                foreach (BoardMovement movement in GetBlockableLine(board, GetTransformedMovementForTeam(0, 1),
                    result => result == MovementAttemptResult.Unblocked, HasMoved ? 1 : 2))
                {
                    yield return movement;
                }

                for (int h = -1; h <= 1; h += 2)
                {
                    BoardVector movement = GetTransformedMovementForTeam(h, 1);
                    if (board.GetMovementAttemptResult(GetMovementFromRelative(movement)) == MovementAttemptResult.OtherTeam)
                    {
                        yield return (BoardMovement)movement;
                    }
                }
            }

            foreach (BoardMovement movement in All())
            {
                if (GetRank(Coordinates.vertical + movement.to.vertical) == PromoteRank)
                {
                    foreach (ChessPieceType piecePromotion in promotablePieces)
                    {
                        yield return new BoardMovement(movement, piecePromotion);
                    }
                }
                else yield return movement;
            }
        }


        protected override Piece InstantiateCopy()
        {
            return new PiecePawn();
        }

    }
}
