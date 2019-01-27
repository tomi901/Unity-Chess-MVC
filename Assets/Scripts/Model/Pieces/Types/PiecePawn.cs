﻿using System.Collections.Generic;

namespace Chess
{
    public class PiecePawn : Piece
    {

        public bool HasMoved { get; private set; } = false;


        protected override void OnMovementDone()
        {
            HasMoved = true;
        }

        protected override IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board)
        {
            foreach (BoardVector movement in GetBlockableLine(board, GetTransformedMovementForTeam(0, 1),
                result => result == MovementAttemptResult.Unblocked, HasMoved ? 1 : 2))
            {
                yield return movement;
            }

            for (int h = -1; h <= 1; h += 2)
            {
                BoardVector movement = GetTransformedMovementForTeam(h, 1);
                if (board.GetMovementAttemptResult(GetMovementToRelative(movement)) == MovementAttemptResult.OtherTeam)
                {
                    yield return movement;
                }
            }
        }

        public override Piece MakeCopy()
        {
            return new PiecePawn() { HasMoved = this.HasMoved };
        }

    }
}
