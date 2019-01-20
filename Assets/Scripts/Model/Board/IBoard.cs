using System.Collections.Generic;

namespace Chess
{

    public interface IBoard : IEnumerable<Tile>
    {

        Tile this[int x, int y] { get; }

        Tile this[BoardVector coords] { get; }

        IEnumerable<Piece> Pieces { get; }

        BoardVector BoardLength { get; }

        void TryToMovePiece(BoardMovement movement);

        bool MovementIsLegal(BoardMovement movement);

    }

}