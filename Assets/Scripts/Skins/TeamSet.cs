using UnityEngine;


namespace Chess
{
    public class PieceSet<T, TeamT> : ScriptableObject
        where TeamT : TeamSet<T>
        where T : Object
    {

        [SerializeField]
        private T defaultPiece;

        [SerializeField]
        private TeamT whiteTeam, blackTeam;

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

        public T GetPiece(PieceTeam team, PieceType type)
        {
            return GetTeam(team)?.GetPiece(type) ?? defaultPiece;
        }

    }

    public class TeamSet<T> where T : Object
    {
        [SerializeField]
        private T pawn, knight, bishop, rook, queen, king;

        public T GetPiece(PieceType piece)
        {
            switch (piece)
            {
                case PieceType.Pawn:
                    return pawn;
                case PieceType.Knight:
                    return knight;
                case PieceType.Bishop:
                    return bishop;
                case PieceType.Rook:
                    return rook;
                case PieceType.Queen:
                    return queen;
                case PieceType.King:
                    return king;
                case PieceType.Unknown:
                default:
                    return null;
            }
        }

    }
}