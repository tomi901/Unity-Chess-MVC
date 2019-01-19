using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public abstract class Piece
    {

        public Tile CurrentTile { get; private set; }

        public IBoard ContainingBoard { get; private set; }

        private BoardVector coordinates;
        public BoardVector Coordinates
        {
            get { return coordinates; }
            set
            {
                if (coordinates == value) return;

                BoardVector prevCoords = coordinates;
                coordinates = value;
                OnCoordinatesChanged(this, new PieceMovementArgs(prevCoords));
            }
        }

        public event EventHandler<PieceMovementArgs> OnCoordinatesChanged = (o, e) => { };

        public PieceTeam Team { get; set; }

        public abstract PieceType Type { get; }


        public void SetBoard(IBoard board, BoardVector newPosition)
        {
            if (board == ContainingBoard) return;

            ContainingBoard = board;
            coordinates = newPosition;
            OnCoordinatesChanged(this, new PieceMovementArgs(board));
        }


        public void MoveTo(BoardVector position, bool checkLegal = true)
        {
            // First if we have to check if the movement is legal, make sure we are moving to a legal position
            if (checkLegal && !GetAllLegalMovements(true).Contains(position - Coordinates))
            {
                return;
            }

            Coordinates = position;
            OnMovementDone();
        }


        public IEnumerable<BoardVector> GetAllLegalMovements(bool relativePosition = false)
        {
            return GetAllLegalMovements(ContainingBoard, relativePosition);
        }

        /// <summary>
        /// Returns all movements in a local way in this piece in a given board that this piece can do.
        /// </summary>
        /// <param name="board">The board this piece is in.</param>
        /// <returns>All valid of movements.</returns>
        public IEnumerable<BoardVector> GetAllLegalMovements(IBoard board, bool relativePosition = false)
        {
            IEnumerable<BoardVector> movements = GetAllPosibleRelativeMovements(board)
                .Where(m => MovementIsLegalInBoard(board, m));

            if (!relativePosition) movements = movements.Select(m => m + Coordinates);

            return movements;
        }

        protected abstract IEnumerable<BoardVector> GetAllPosibleRelativeMovements(IBoard board);


        protected virtual BoardVector GetTransformedMovementForTeam(int horizontal, int vertical)
        {
            return GetTransformedMovementForTeam(new BoardVector(horizontal, vertical));
        }

        protected virtual BoardVector GetTransformedMovementForTeam(BoardVector movement)
        {
            switch (Team)
            {
                case PieceTeam.Black:
                    movement.vertical = -movement.vertical;
                    return movement;
                case PieceTeam.White:
                    return movement;
                case PieceTeam.Unknown:
                default:
                    throw new Exception($"Invalid team ({Team}) to transform movement \"{movement}\"");
            }
        }


        /// <summary>
        /// Returns a sequence of movements in a straigth line, if the line is blocked (Not valid)
        /// it stops returning movements.
        /// </summary>
        /// <param name="board">The board this piece is in.</param>
        /// <param name="step">Where each step goes</param>
        /// <returns>Sequence of movements.</returns>
        protected IEnumerable<BoardVector> GetBlockableLine(IBoard board, BoardVector step)
        {
            BoardVector movement = step;
            while (MovementIsLegalInBoard(board, movement))
            {
                yield return movement;
                movement += step;
            }
        }


        protected IEnumerable<BoardVector> GetHorizontalAndVerticalLines(IBoard board)
        {
            foreach (BoardVector movement in GetBlockableLine(board, new BoardVector(0, 1)))
            {
                yield return movement;
            }
            foreach (BoardVector movement in GetBlockableLine(board, new BoardVector(1, 0)))
            {
                yield return movement;
            }
            foreach (BoardVector movement in GetBlockableLine(board, new BoardVector(-1, 0)))
            {
                yield return movement;
            }
            foreach (BoardVector movement in GetBlockableLine(board, new BoardVector(0, -1)))
            {
                yield return movement;
            }
        }

        protected IEnumerable<BoardVector> GetDiagonalLines(IBoard board)
        {
            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    foreach (BoardVector movement in GetBlockableLine(board, new BoardVector(x, y)))
                    {
                        yield return movement;
                    }
                }
            }
        }


        public bool MovementIsLegalInBoard(IBoard board, BoardVector movement)
        {
            return board.MovementIsLegal(this, Coordinates + movement);
        }


        protected virtual void OnMovementDone()
        {

        }

    }
}
