using UnityEngine;


namespace Chess
{
    public class PieceSet<T, TeamT> : ScriptableObject
        where TeamT : TeamSet<T>
        where T : Object
    {

        [SerializeField]
        private T defaultPiece = default;

        [SerializeField]
        private TeamT whiteTeam = default, blackTeam = default;

        public TeamT GetTeam(PieceTeam team)
        {
            switch (team)
            {
                case PieceTeam.White:
                    return whiteTeam;
                case PieceTeam.Black:
                    return blackTeam;
                case PieceTeam.Unknown:
                default:
                    return null;
            }
        }

        public T GetPiece(Piece piece)
        {
            return GetTeam(piece.Team)?.GetPiece(piece) ?? defaultPiece;
        }

    }

    public class TeamSet<T> where T : Object
    {
        [SerializeField]
        private T pawn = default, knight = default, bishop = default, rook = default, queen = default, king = default;

        public T GetPiece(Piece piece)
        {
            switch (piece)
            {
                case PiecePawn p:
                    return pawn;
                case PieceKnight p:
                    return knight;
                case PieceBishop p:
                    return bishop;
                case PieceRook p:
                    return rook;
                case PieceQueen p:
                    return queen;
                case PieceKing p:
                    return king;
                default:
                    return null;
            }
        }

    }
}