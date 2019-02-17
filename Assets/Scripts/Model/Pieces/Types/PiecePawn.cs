using System.Collections.Generic;

namespace Chess
{
    public class PiecePawn : Piece
    {

        protected readonly ChessPieceType[] promotablePieces =
        {
            ChessPieceType.Rook, ChessPieceType.Knight, ChessPieceType.Bishop, ChessPieceType.Queen
        };


        public bool HasMoved { get; private set; } = false;


        private int PromotionHeight
        {
            get
            {
                switch (Team)
                {
                    case PieceTeam.White:
                        return ContainingBoard.BoardLength.vertical - 1;
                    case PieceTeam.Black:
                        return 0;
                    default:
                        return -1;
                }
            }
        }


        protected override void OnMovementDone()
        {
            HasMoved = true;
        }

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

            int promotionHeight = PromotionHeight;
            foreach (BoardMovement movement in All())
            {
                if ((Coordinates.vertical + movement.to.vertical) == promotionHeight)
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
            return new PiecePawn() { HasMoved = this.HasMoved };
        }

    }
}
