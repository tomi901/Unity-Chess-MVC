using System.Collections.Generic;

namespace Chess
{
    public class PiecePawn : Piece
    {

        public override PieceType Type => PieceType.Pawn;

        public bool HasMoved { get; private set; } = false;


        protected override void OnMovementDone()
        {
            HasMoved = true;
        }

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(IBoard board)
        {
            foreach (BoardVector movement in GetBlockableLine(board, GetTransformedMovementForTeam(0, 1),
                HasMoved ? 1 : 2))
            {
                yield return movement;
            }
        }

    }
}
