using System.Collections.Generic;

namespace Chess
{
    public interface IBoard : IEnumerable<Tile>
    {

        Tile this[int x, int y] { get; }

        Tile this[BoardVector coords] { get; }

        IEnumerable<Piece> Pieces { get; }

        BoardVector BoardLength { get; }

        bool TryToMovePiece(BoardMovement movement);

        MovementAttemptResult GetMovementAttemptResult(BoardMovement movement);

    }
}