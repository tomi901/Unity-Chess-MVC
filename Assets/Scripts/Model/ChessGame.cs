using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class ChessGame
    {

        public const int StandardBoardSize = 8;
        public static BoardVector StandardBoardVectorSize => new BoardVector(StandardBoardSize);

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


        private PieceTeam currentTurn;
        public PieceTeam CurrentTurnTeam
        {
            get => currentTurn;
            private set
            {
                if (value == currentTurn) return;

                currentTurn = value;
                OnTurnChange(this, EventArgs.Empty);
            }
        }

        public Board Board { get; private set; }


        public event EventHandler OnTurnChange = (o, e) => { };


        public ChessGame(BoardVector boardSize, IEnumerable<IPieceEntry> startingPieces,
            PieceTeam startingTurn = PieceTeam.White)
        {
            Board = new Board(boardSize, startingPieces)
            {
                UsedForGame = this
            };
            CurrentTurnTeam = startingTurn;
        }

        public static ChessGame StartNew()
        {
            return new ChessGame(StandardBoardVectorSize, standardStartingPieces);
        }


        public MovementAttemptResult GetMovementAttemptResult(BoardMovement movement)
        {
            Piece movingPiece = Board[movement.from].CurrentPiece;
            if (!PieceIsInTurn(movingPiece))
            {
                return MovementAttemptResult.NotInTurn;
            }

            Piece pieceInTargetPos = Board[movement.to].CurrentPiece;
            switch (pieceInTargetPos)
            {
                case null:
                    return MovementAttemptResult.Unblocked;
                default:
                    return pieceInTargetPos.Team == movingPiece.Team ?
                                MovementAttemptResult.SameTeam :
                                MovementAttemptResult.OtherTeam;
            }
        }


        public bool TryToDoMovement(BoardMovement movement)
        {
            Piece currentPiece = Board[movement.from].CurrentPiece;
            // TODO: Check if the movement is in the current legal movements cached list
            if (currentPiece != null && PieceIsInTurn(currentPiece) &&
                currentPiece.GetAllLegalMovements(Board).Contains(movement.to))
            {
                Board[movement.to].CurrentPiece = currentPiece;
                NextTurn();
                return true;
            }
            else return false;
        }

        private void NextTurn()
        {
            switch (CurrentTurnTeam)
            {
                case PieceTeam.White:
                    CurrentTurnTeam = PieceTeam.Black;
                    break;
                default:
                case PieceTeam.Black:
                    CurrentTurnTeam = PieceTeam.White;
                    break;
            }
        }

        public bool PieceIsInTurn(Piece piece)
        {
            return piece.Team == CurrentTurnTeam;
        }

    }
}
