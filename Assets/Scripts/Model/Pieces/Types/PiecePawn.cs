using System.Collections.Generic;

namespace Chess
{
    public class PiecePawn : Piece
    {

        public const int PromoteRank = 8;

        protected static readonly ChessPieceType[] promotablePieces =
        {
            ChessPieceType.Rook, ChessPieceType.Knight, ChessPieceType.Bishop, ChessPieceType.Queen
        };


        public bool LastMovementWasDouble { get; protected set; }


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

                    // Check for a pawn in that side and if it moved to spaces to
                    // see if the En Passant rule is applyable
                    BoardVector sidePosition = Coordinates + new BoardVector(h, 0);
                    bool passantRule = board.PositionIsInside(sidePosition) && 
                        board[sidePosition].CurrentPiece is PiecePawn pawn &&
                        pawn.EnPassantRuleCapturable(this);

                    // Check for En Passant rule or regular capture
                    if (passantRule || (board.GetMovementAttemptResult(GetMovementFromRelative(movement)) == 
                        MovementAttemptResult.OtherTeam))
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


        protected bool EnPassantRuleCapturable(PiecePawn capturedBy)
        {
            return this.Team != capturedBy.Team && this.LastMovementWasDouble;
        }


        protected override void OnMovementDone(BoardVector deltaMovement)
        {
            LastMovementWasDouble = System.Math.Abs(deltaMovement.vertical) == 2;

            // After a diagonal movement we check one tile behind to check if we should apply
            // En Passant rule
            if (deltaMovement.horizontal != 0 && ContainingBoard[Coordinates - GetTransformedMovementForTeam(0, 1)]
                .CurrentPiece is PiecePawn pawn && pawn.EnPassantRuleCapturable(this))
            {
                pawn.Capture(this);
            }
        }


        protected override Piece InstantiateCopy()
        {
            return new PiecePawn()
            {
                LastMovementWasDouble = this.LastMovementWasDouble
            };
        }

    }
}
