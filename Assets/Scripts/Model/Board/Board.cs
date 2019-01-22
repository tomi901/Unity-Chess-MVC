using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{

    public class Board : IEnumerable<Tile>
    {

        public ChessGame UsedForGame { get; set; }

        private readonly Tile[,] tiles;

        public Tile this[BoardVector coords]
        {
            get => this[coords.horizontal, coords.vertical];
            private set => this[coords.horizontal, coords.vertical] = value;
        }

        public Tile this[int horizontal, int vertical]
        {
            get => tiles[horizontal, vertical];
            private set => tiles[horizontal, vertical] = value;
        }

        public BoardVector BoardLength => new BoardVector(tiles.GetLength(0), tiles.GetLength(1));


        public IEnumerable<Piece> Pieces => this.Select(tile => tile.CurrentPiece).Where(piece => piece != null);


        public Board(BoardVector size)
        {
            tiles = new Tile[size.horizontal, size.vertical];

            foreach (BoardVector pos in BoardVector.GetRange(size))
            {
                this[pos] = new Tile(this, pos);
            }
        }

        public Board(BoardVector size, IEnumerable<IPieceEntry> pieceEntries) : this(size)
        {
            foreach (PiecePlacement placement in pieceEntries.SelectMany(entry => entry.GetPiecePlacements(this)))
            {
                PlacePiece(placement.piece, placement.atPosition);
            }
        }

        public void PlacePiece(Piece piece, BoardVector position)
        {
            if (this[position].HasPiece)
            {
                throw new System.InvalidOperationException($"Can't place piece '{piece}': " +
                    $"There's overlapping pieces at position " +
                    $"'{position.ToStringCoordinates(true)}'");
            }

            this[position].CurrentPiece = piece;
        }


        public bool TryToMovePiece(BoardMovement movement)
        {
            return UsedForGame.TryToDoMovement(movement);
        }


        public bool PositionIsInside(BoardVector position)
        {
            return position.IsInsideBox(BoardLength);
        }


        public MovementAttemptResult GetMovementAttemptResult(BoardMovement movement)
        {
            if (!PositionIsInside(movement.from) || !PositionIsInside(movement.to)) return MovementAttemptResult.Unvalid;

            return UsedForGame.GetMovementAttemptResult(movement);
        }


        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (Tile tile in tiles)
            {
                yield return tile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
