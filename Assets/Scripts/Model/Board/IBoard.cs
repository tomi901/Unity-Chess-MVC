using System.Collections.Generic;

namespace Chess
{

    public interface IBoard : IEnumerable<Piece>
    {

        Piece this[int x, int y] { get; }

        Piece this[BoardVector coords] { get; }

        BoardVector BoardLength { get; }

        bool MovementIsLegal(Piece movingPiece, BoardVector toPosition);

    }

}