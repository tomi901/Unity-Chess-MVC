using System;
using System.Collections.Generic;


namespace Chess
{

    public struct PiecePlacement
    {
        public readonly Piece piece;
        public readonly BoardVector atPosition;

        public PiecePlacement(Piece piece, BoardVector atPosition)
        {
            this.piece = piece ?? throw new ArgumentNullException(nameof(piece));
            this.atPosition = atPosition;
        }
    }

    public interface IPieceEntry
    {
        IEnumerable<PiecePlacement> GetPiecePlacements(Board forBoard);
    }

    public abstract class PieceEntry<PieceT> : IPieceEntry where PieceT : Piece, new()
    {

        public readonly PieceTeam team;

        public abstract IEnumerable<PiecePlacement> GetPiecePlacements(Board forBoard);

        public PieceEntry(PieceTeam team)
        {
            this.team = team;
        }

        protected PiecePlacement GetNewPlacement(BoardVector atPosition)
        {
            return new PiecePlacement(new PieceT() { Team = team }, atPosition);
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

        public override IEnumerable<PiecePlacement> GetPiecePlacements(Board forBoard)
        {
            yield return GetNewPlacement(position);
        }
    }

    public class RowPiecesEntry<PieceT> : PieceEntry<PieceT> where PieceT : Piece, new()
    {

        public readonly int height;

        public RowPiecesEntry(PieceTeam team, int height) : base(team)
        {
            this.height = height;
        }

        public override IEnumerable<PiecePlacement> GetPiecePlacements(Board forBoard)
        {
            int hLength = forBoard.BoardLength.horizontal;
            for (int h = 0; h < hLength; h++)
            {
                yield return GetNewPlacement(new BoardVector(h, height));
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

        public override IEnumerable<PiecePlacement> GetPiecePlacements(Board forBoard)
        {
            yield return GetNewPlacement(position);
            yield return GetNewPlacement(new BoardVector(forBoard.BoardLength.horizontal - position.horizontal - 1,
                position.vertical));
        }
    }

}