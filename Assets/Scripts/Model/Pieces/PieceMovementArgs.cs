using System;

namespace Chess
{
    public class PieceMovementArgs : EventArgs
    {

        public readonly BoardVector previousPosition;
        public readonly BoardVector toPosition;

        public readonly IBoard movedToBoard = null;
        public bool HasMovedToAnotherBoard => movedToBoard != null;

        public PieceMovementArgs(BoardVector previousPosition)
        {
            this.previousPosition = previousPosition;
        }

        public PieceMovementArgs(IBoard moveToBoard)
        {
            movedToBoard = moveToBoard;
        }

    }
}
