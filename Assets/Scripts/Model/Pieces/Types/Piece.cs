using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public abstract class Piece
    {

        private Tile currentTile;
        public Tile CurrentTile
        {
            get => currentTile;
            internal set
            {
                if (value == currentTile) return;

                var moveArgs = new PieceMovementArgs(currentTile, value);
                currentTile = value;

                currentTile?.CurrentPiece?.Capture(this);

                if (!moveArgs.HasMovedToAnotherBoard) OnMovementDone();
                OnCoordinatesChanged(this, moveArgs);
            }
        }

        public Board ContainingBoard => CurrentTile.Board;
        public Turn CurrentTurn => ContainingBoard.UsedInTurn;
        public BoardVector Coordinates => CurrentTile?.Coordinates ?? default;


        public PieceTeam Team { get; set; }


        public event EventHandler<PieceMovementArgs> OnCoordinatesChanged = (o, e) => { };
        public event EventHandler<PieceSetTargetMovementArgs> OnMovementTargetSet = (o, e) => { };
        public event EventHandler<EventArgs> OnCapture = (o, e) => { };


        public BoardMovement GetMovementTo(BoardVector toPosition)
        {
            return new BoardMovement(Coordinates, toPosition);
        }

        public BoardMovement GetMovementToRelative(BoardVector relativeMovement)
        {
            return new BoardMovement(Coordinates, Coordinates + relativeMovement);
        }


        private void Capture(Piece byPiece)
        {
            OnCapture(this, EventArgs.Empty);
        }


        public void TryToMoveTo(BoardVector position)
        {
            ContainingBoard.TryToMovePiece(GetMovementTo(position));
        }


        protected virtual void OnMovementDone()
        {

        }

        public IEnumerable<BoardMovement> GetAllLegalMovements()
        {
            return GetAllLegalMovements(true).Select(GetMovementToRelative);
        }


        /// <summary>
        /// Returns all movements in a local way in this piece in a given board that this piece can do.
        /// </summary>
        /// <param name="board">The board this piece is in.</param>
        /// <returns>All valid of movements.</returns>
        private IEnumerable<BoardVector> GetAllLegalMovements(bool relativePosition)
        {
            if (!CurrentTurn.IsInTurn(this))
                return Enumerable.Empty<BoardVector>();

            Board board = ContainingBoard;
            IEnumerable<BoardVector> movements = GetAllPosibleRelativeMovements(board)
                .Where(m => GetMovementAttemptResult(board, m).IsUnblockedOrOtherTeam());

            if (!relativePosition) movements = movements.Select(m => m + Coordinates);

            return movements;
        }

        protected abstract IEnumerable<BoardVector> GetAllPosibleRelativeMovements(Board board);


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


        protected IEnumerable<BoardVector> GetBlockableLine(Board board, BoardVector step, int maxSteps = -1)
        {
            return GetBlockableLine(board, step, MovementAttemptResults.IsUnblockedOrOtherTeam, maxSteps);
        }

        /// <summary>
        /// Returns a sequence of movements in a straigth line, if the line is blocked (Not valid)
        /// it stops returning movements.
        /// </summary>
        /// <param name="board">The board this piece is in.</param>
        /// <param name="step">Where each step goes</param>
        /// <returns>Sequence of movements.</returns>
        protected IEnumerable<BoardVector> GetBlockableLine(Board board, BoardVector step, 
            Predicate<MovementAttemptResult> resultFilter, int maxSteps = -1)
        {
            if (resultFilter == null) yield break;

            int steps = 0;
            BoardVector movement = step;
            MovementAttemptResult attemptResult;
            while (resultFilter(attemptResult = GetMovementAttemptResult(board, movement)))
            {
                yield return movement;
                movement += step;

                if ((maxSteps >= 0 && ++steps >= maxSteps) || attemptResult == MovementAttemptResult.OtherTeam)
                    yield break;
            }
        }


        protected IEnumerable<BoardVector> GetHorizontalAndVerticalLines(Board board)
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

        protected IEnumerable<BoardVector> GetDiagonalLines(Board board)
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


        public MovementAttemptResult GetMovementAttemptResult(Board board, BoardVector movement)
        {
            return board.GetMovementAttemptResult(GetMovementToRelative(movement));
        }


        public Piece MakeCopy()
        {
            Piece newPiece = InstantiateCopy();
            newPiece.Team = Team;
            return newPiece;
        }

        protected abstract Piece InstantiateCopy();

    }
}
