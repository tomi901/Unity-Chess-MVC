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
            yield return GetTransformedMovementForTeam(0, 1);
            
            if (!HasMoved) yield return GetTransformedMovementForTeam(0, 2);
        }

    }
}
