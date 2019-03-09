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

                currentTurn.OnChange += OnTurnChangeListener;
            }
        }

        public int CurrentTurnNumber => CurrentTurn.Number;
        public PieceTeam CurrentTurnTeam => CurrentTurn.Team;

        public int CurrentTurnDrawMovements => CurrentTurn.DrawMovements;

        public bool CanUndoTurn => CurrentTurn.HasPrevious;

        public PieceTeam CurrentTurnCheck => CurrentTurn.CurrentCheckedTeam;
        public PieceTeam NextTurnsCheck => CurrentTurn.FilteredNextTurnsCheck;
        public PieceTeam CurrentWinner => CurrentTurn.WinnerTeam;


        public Board Board => CurrentTurn.Board;


        public bool AllowCastling => true;
        public bool AllowEnPassant => true;

        public int DrawMovementsQuantity { get; } = 50;
        public bool UseDrawMovements => DrawMovementsQuantity > 0;

        public event EventHandler OnTurnChange = (o, e) => { };
        public event EventHandler OnGameEnded = (o, e) => { };


        public static ChessGame StartNew()
        {
            return new ChessGame(StandardBoardVectorSize, standardStartingPieces, StandardStartingTeam);
        }

        public ChessGame(BoardVector boardSize, IEnumerable<IPieceEntry> startingPieces, PieceTeam startingTurn)
        {
            CurrentTurn = new Turn(this, new Board(boardSize, startingPieces), startingTurn);
        }


        public bool CanDoMovementInCurrentTurn(BoardMovement movement) => currentTurn.CanDoMovement(movement);


        public ICollection<BoardMovement> GetAllMovements() => currentTurn.AllPossibleMovements.Keys;

        public IEnumerable<BoardMovement> GetAllMovementsFromTileInCurrentTurn(BoardVector fromTile)
        {
            return currentTurn.GetAllMovementsFrom(fromTile);
        }

        public IEnumerable<BoardMovement> GetAllMovementsToTileInCurrentTurn(BoardVector toTile)
        {
            return currentTurn.GetAllMovementsTo(toTile);
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
            else return PieceTeam.None;
        }


        public bool TryToDoMovement(BoardMovement movement)
        {
            Piece currentPiece = Board[movement.from].CurrentPiece;
            if (currentPiece != null && CurrentTurn.CanDoMovement(movement))
            {
                CurrentTurn.Next(movement);
                return true;
            }
            else return false;
        }


        public void UndoTurn() => CurrentTurn.Undo();


        private void OnTurnChangeListener(object sender, EventArgs eventArgs)
        {
            if (!CurrentTurn.CachedMovementsAreFiltered)
                CurrentTurn.FilterCachedMovements();

            OnTurnChange(sender, eventArgs);

            if (!CurrentTurn.HasMovementsLeft)
            {
                // Current team loses
                OnGameEnded(this, EventArgs.Empty);
            }
        }

    }
}
