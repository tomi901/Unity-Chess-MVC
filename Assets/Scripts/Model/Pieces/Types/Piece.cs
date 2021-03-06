﻿using System;
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

                Tile previousTile = currentTile;
                var moveArgs = new PieceMovementArgs(previousTile, value);
                currentTile = value;

                currentTile?.CurrentPiece?.Capture(this);

                if (!moveArgs.HasMovedToAnotherBoard)
                {
                    MovementsDone++;
                    CurrentTurn.SetPieceMoved(this);

                    OnMovementDone(Coordinates - previousTile.Coordinates);
                }
                OnCoordinatesChanged(this, moveArgs);
            }
        }

        public Board ContainingBoard => CurrentTile.Board;

        public Turn CurrentTurn => ContainingBoard.UsedInTurn;
        public bool IsInTurn => CurrentTurn.IsInTurn(this);

        public BoardVector Coordinates => CurrentTile?.Coordinates ?? new BoardVector(-1, -1);
        public int Rank => GetRank(Coordinates.vertical);


        public int MovementsDone { get; private set; } = 0;
        public bool HasMoved => MovementsDone > 0;


        public PieceTeam Team { get; set; }


        public event EventHandler<PieceMovementArgs> OnCoordinatesChanged = (o, e) => { };
        public event EventHandler OnCapture = (o, e) => { };


        public int GetRank(int verticalCoord)
        {
            switch (Team)
            {
                case PieceTeam.White:
                    return verticalCoord + 1;
                case PieceTeam.Black:
                    return ContainingBoard.BoardLength.vertical - verticalCoord;
                default:
                    return -1;
            }
        }


        public BoardMovement GetMovementTo(BoardVector toPosition)
        {
            return new BoardMovement(Coordinates, toPosition);
        }

        public BoardMovement GetMovementFromRelative(BoardVector relativeMovement)
        {
            return new BoardMovement(Coordinates, Coordinates + relativeMovement);
        }

        public BoardMovement GetMovementFromRelative(BoardMovement relativeMovement)
        {
            return relativeMovement + Coordinates;
        }


        protected void Capture(Piece byPiece)
        {
            CurrentTurn.SetPieceCaptured(this);
            CurrentTile.ClearPiece();

            OnCapture(this, EventArgs.Empty);
        }


        protected virtual void OnMovementDone(BoardVector deltaMovement) { }


        /// <summary>
        /// Returns all movements in the current board that this piece can do.
        /// </summary>
        /// <returns>All valid movements for this piece.</returns>
        public IEnumerable<BoardMovement> GetAllLegalMovements()
        {
            if (IsInTurn)
            {
                Board board = ContainingBoard;
                return GetAllPosibleRelativeMovements(board)
                    .Where(m => GetMovementAttemptResult(board, m.to).IsUnblockedOrOtherTeam())
                    .Select(GetMovementFromRelative);
            }
            else return Enumerable.Empty<BoardMovement>();
        }

        protected abstract IEnumerable<BoardMovement> GetAllPosibleRelativeMovements(Board board);


        public IEnumerable<BoardMovement> GetAllExtraMovements()
        {
            return IsInTurn ?
                GetAllExtraRelativeMovements(ContainingBoard).Select(GetMovementFromRelative) :
                Enumerable.Empty<BoardMovement>();
        }

        protected virtual IEnumerable<BoardMovement> GetAllExtraRelativeMovements(Board board) => 
            Enumerable.Empty<BoardMovement>();



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
                case PieceTeam.None:
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


        protected IEnumerable<BoardMovement> GetHorizontalAndVerticalLines(Board board)
        {
            foreach (BoardMovement movement in GetBlockableLine(board, new BoardVector(0, 1)))
            {
                yield return movement;
            }
            foreach (BoardMovement movement in GetBlockableLine(board, new BoardVector(1, 0)))
            {
                yield return movement;
            }
            foreach (BoardMovement movement in GetBlockableLine(board, new BoardVector(-1, 0)))
            {
                yield return movement;
            }
            foreach (BoardMovement movement in GetBlockableLine(board, new BoardVector(0, -1)))
            {
                yield return movement;
            }
        }

        protected IEnumerable<BoardMovement> GetDiagonalLines(Board board)
        {
            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    foreach (BoardMovement movement in GetBlockableLine(board, new BoardVector(x, y)))
                    {
                        yield return movement;
                    }
                }
            }
        }


        public MovementAttemptResult GetMovementAttemptResult(Board board, BoardVector relativeMovement)
        {
            return board.GetMovementAttemptResult(GetMovementFromRelative(relativeMovement));
        }


        public Piece MakeCopy()
        {
            Piece newPiece = InstantiateCopy();
            newPiece.Team = this.Team;
            newPiece.MovementsDone = this.MovementsDone;
            return newPiece;
        }

        protected abstract Piece InstantiateCopy();

    }
}
