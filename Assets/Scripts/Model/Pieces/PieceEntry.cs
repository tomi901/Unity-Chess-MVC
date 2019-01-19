using System.Collections.Generic;


namespace Chess
{

    public interface IPieceEntry
    {
        IEnumerable<Piece> GetPieces(IBoard forBoard);
    }

    public abstract class PieceEntry<PieceT> : IPieceEntry where PieceT : Piece, new()
    {

        public readonly PieceTeam team;

        public abstract IEnumerable<Piece> GetPieces(IBoard forBoard);

        public PieceEntry(PieceTeam team)
        {
            this.team = team;
        }

        protected PieceT GetNewPiece(BoardVector position)
        {
            return new PieceT
            {
                Coordinates = position,
                Team = team
            };
        }

    }

    public class SinglePieceEntry<PieceT> : PieceEntry<PieceT> where PieceT : Piece, new()
    {

        public readonly BoardVector position;

        public SinglePieceEntry(PieceTeam team, int horizontal, int vertical) 
            : this(team, new BoardVector(horizontal, vertical))
        {
        }

        public SinglePieceEntry(PieceTeam team, BoardVector startPosition) : base(team)
        {
            this.position = startPosition;
        }

        public override IEnumerable<Piece> GetPieces(IBoard forBoard)
        {
            yield return GetNewPiece(position);
        }
    }

    public class RowPiecesEntry<PieceT> : PieceEntry<PieceT> where PieceT : Piece, new()
    {

        public readonly int height;

        public RowPiecesEntry(PieceTeam team, int height) : base(team)
        {
            this.height = height;
        }

        public override IEnumerable<Piece> GetPieces(IBoard forBoard)
        {
            int hLength = forBoard.BoardLength.horizontal;
            for (int h = 0; h < hLength; h++)
            {
                yield return GetNewPiece(new BoardVector(h, height));
            }
        }
    }

    public class MirroredPiecesEntry<PieceT> : PieceEntry<PieceT> where PieceT : Piece, new()
    {

        public readonly BoardVector position;

        public MirroredPiecesEntry(PieceTeam team, int horizontal, int vertical) 
            : this(team, new BoardVector(horizontal, vertical))
        {
        }

        public MirroredPiecesEntry(PieceTeam team, BoardVector position) : base(team)
        {
            this.position = position;
        }

        public override IEnumerable<Piece> GetPieces(IBoard forBoard)
        {
            yield return GetNewPiece(position);
            yield return GetNewPiece(new BoardVector(forBoard.BoardLength.horizontal - position.horizontal - 1,
                position.vertical));
        }
    }

}