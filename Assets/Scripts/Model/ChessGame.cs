

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


        public PieceTeam CurrentTurn { get; private set; }

        public Board Board { get; private set; }


        public static ChessGame StartNew()
        {
            ChessGame newGame = new ChessGame
            {
                Board = new Board(StandardBoardVectorSize, standardStartingPieces),
                CurrentTurn = PieceTeam.White,
            };

            newGame.Board.UsedForGame = newGame;

            return newGame;
        }

        public bool MovementIsLegal(BoardMovement movement)
        {
            Piece movingPiece = Board[movement.from].CurrentPiece;
            Piece pieceInTargetPos = Board[movement.to].CurrentPiece;

            return pieceInTargetPos == null || pieceInTargetPos.Team != movingPiece.Team;
        }

    }
}
