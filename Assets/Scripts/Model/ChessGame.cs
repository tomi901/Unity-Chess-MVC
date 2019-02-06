using System;
using System.Collections.Generic;

namespace Chess
{
    public class ChessGame
    {

        public const int StandardBoardSize = 8;
        public static BoardVector StandardBoardVectorSize => new BoardVector(StandardBoardSize);
        public const PieceTeam StandardStartingTeam = PieceTeam.White;

        private static readonly IPieceEntry[] standardStartingPieces =
        {
            // White team
            new RowPiecesEntry<PiecePawn>(PieceTeam.White, 1),
            new MirroredPiecesEntry<PieceRook>(PieceTeam.White, 0, 0),
            new MirroredPiecesEntry<PieceKnight>(PieceTeam.White, 1, 0),
            new MirroredPiecesEntry<PieceBishop>(PieceTeam.White, 2, 0),
            new SinglePieceEntry<PieceQueen>(PieceTeam.White, 3, 0),
            new SinglePieceEntry<PieceKing>(PieceTeam.White, 4, 0),
            // Black team
            new RowPiecesEntry<PiecePawn>(PieceTeam.Black, 6),
            new MirroredPiecesEntry<PieceRook>(PieceTeam.Black, 0, 7),
            new MirroredPiecesEntry<PieceKnight>(PieceTeam.Black, 1, 7),
            new MirroredPiecesEntry<PieceBishop>(PieceTeam.Black, 2, 7),
            new SinglePieceEntry<PieceQueen>(PieceTeam.Black, 3, 7),
            new SinglePieceEntry<PieceKing>(PieceTeam.Black, 4, 7),
        };

        private Turn currentTurn;
        public Turn CurrentTurn
        {
            get => currentTurn;
            set
            {
                if (value == currentTurn) return;

                currentTurn = value;

                currentTurn.CacheCurrentPossibleMovements();
                currentTurn.FilterCachedMovements();

                currentTurn.OnNext += OnNextTurn;
            }
        }

        public int CurrentTurnNumber => currentTurn.Number;
        public PieceTeam CurrentTurnTeam => currentTurn.Team;


        public Board Board => CurrentTurn.Board;


        public event EventHandler OnTurnChange = (o, e) => { };


        public static ChessGame StartNew()
        {
            return new ChessGame(StandardBoardVectorSize, standardStartingPieces, StandardStartingTeam);
        }

        public ChessGame(BoardVector boardSize, IEnumerable<IPieceEntry> startingPieces, PieceTeam startingTurn)
        {
            CurrentTurn = new Turn(this, new Board(boardSize, startingPieces), startingTurn);
        }


        public bool CanDoMovementInCurrentTurn(BoardMovement movement)
        {
            return currentTurn.CanDoMovement(movement);
        }

        public IEnumerable<BoardVector> GetAllMovementsForTileInCurrentTurn(BoardVector fromTile)
        {
            return currentTurn.GetAllMovementTargetsFrom(fromTile);
        }


        public MovementAttemptResult GetMovementAttemptResult(BoardMovement movement, Board board)
        {
            Piece pieceInTargetPos = board[movement.to].CurrentPiece;
            switch (pieceInTargetPos)
            {
                case null:
                    return MovementAttemptResult.Unblocked;
                default:
                    return pieceInTargetPos.Team == board[movement.from].CurrentPiece.Team ?
                                MovementAttemptResult.SameTeam :
                                MovementAttemptResult.OtherTeam;
            }
        }


        public PieceTeam GetCheckResult(BoardMovement movement, Board inBoard)
        {
            Piece capturedPiece = inBoard[movement.to].CurrentPiece;
            if (capturedPiece != null && capturedPiece is PieceKing)
            {
                return capturedPiece.Team;
            }
            else return PieceTeam.Unknown;
        }


        public bool TryToDoMovement(BoardMovement movement)
        {
            Piece currentPiece = Board[movement.from].CurrentPiece;
            if (currentPiece != null && currentTurn.CanDoMovement(movement))
            {
                Board[movement.to].CurrentPiece = currentPiece;
                CurrentTurn.Next(movement);
                // TODO: Check if we have no movements left, if not,
                // trigger a Tie or a Win depending if we have a Checkmate or not
                if (CurrentTurn.AllPossibleMovements.Count <= 0)
                {
                    // Current team loses
                }

                return true;
            }
            else return false;
        }


        private void OnNextTurn(object sender, EventArgs eventArgs)
        {
            CurrentTurn.FilterCachedMovements();
            OnTurnChange(sender, eventArgs);
        }

    }
}
