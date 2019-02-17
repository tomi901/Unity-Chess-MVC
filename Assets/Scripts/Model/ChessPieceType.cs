

namespace Chess
{
    public enum ChessPieceType
    {
        None = 0,
        Pawn,
        Rook,
        Bishop,
        Knight,
        Queen,
        King,
    }

    public static class ChessPieces
    {

        public static Piece InstantiateNew(ChessPieceType pieceType, PieceTeam team)
        {
            Piece newPiece = InstantiateNew(pieceType);
            newPiece.Team = team;
            return newPiece;
        }

        public static Piece InstantiateNew(ChessPieceType pieceType)
        {
            switch (pieceType)
            {
                case ChessPieceType.Pawn:
                    return new PiecePawn();
                case ChessPieceType.Rook:
                    return new PieceRook();
                case ChessPieceType.Bishop:
                    return new PieceBishop();
                case ChessPieceType.Knight:
                    return new PieceKnight();
                case ChessPieceType.Queen:
                    return new PieceQueen();
                case ChessPieceType.King:
                    return new PieceKing();
                case ChessPieceType.None:
                default:
                    throw new System.ArgumentException($"Invalid piece to instantiate ({pieceType}).", nameof(pieceType));
            }
        }

    }
}