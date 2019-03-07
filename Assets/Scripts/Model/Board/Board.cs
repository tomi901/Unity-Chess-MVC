using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{

    public class Board : IEnumerable<Tile>
    {

        public Turn UsedInTurn { get; internal set; }

        public ChessGame UsedForGame => UsedInTurn.ForGame;

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
        public int PieceCount => this.Count(tile => tile.HasPiece);


        public event EventHandler<Piece> OnPieceAdded = (o, e) => { };
        /// <summary>
        /// When the entire board changes, so we can delete and re-create all pieces in the view.
        /// </summary>
        public event EventHandler OnBoardChanged = (o, e) => { };


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

        public Board(BoardVector size, IEnumerable<Piece> copyPieces) : this(size)
        {
            foreach (Piece piece in copyPieces)
            {
                PlacePiece(piece.MakeCopy(), piece.Coordinates);
            }
        }


        public void SetBoard(Board board)
        {
            ClearAllPieces();
            foreach (Piece piece in board.Pieces)
            {
                PlacePiece(piece.MakeCopy(), piece.Coordinates);
            }

            OnBoardChanged(this, EventArgs.Empty);
        }


        public void PlacePiece(Piece piece, BoardVector position, bool checkEmptySpace = true)
        {
            if (checkEmptySpace && this[position].HasPiece)
            {
                throw new InvalidOperationException($"Can't place piece '{piece}': " +
                    $"There's overlapping pieces at position " +
                    $"'{position.ToStringCoordinates(true)}'");
            }

            this[position].CurrentPiece = piece;
            OnPieceAdded(this, piece);
        }

        private void ClearAllPieces()
        {
            foreach (Tile tile in tiles)
            {
                tile.ClearPiece();
            }
        }


        public void DoMovement(BoardMovement movement)
        {
            if (!MovementIsInside(movement))
                throw new ArgumentOutOfRangeException(nameof(movement));

            Piece movingPiece = this[movement.from].CurrentPiece;
            this[movement.to].CurrentPiece = movingPiece;
            if (movement.Promotion != ChessPieceType.None)
            {
                PlacePiece(ChessPieces.InstantiateNew(movement.Promotion, movingPiece.Team), movement.to, false);
            }
        }


        public Piece Raycast(BoardVector from, BoardVector direction)
        {
            if (direction.IsZero || !PositionIsInside(from))
                return null;

            direction = direction.Normalized;
            for (BoardVector current = from + direction; PositionIsInside(current); current += direction)
            {
                Piece piece = this[current].CurrentPiece;
                if (piece != null)
                    return piece;
            }
            return null;
        }


        public bool TryToMovePiece(BoardMovement movement) => UsedForGame.TryToDoMovement(movement);


        public bool PositionIsInside(BoardVector position) => position.IsInsideBox(BoardLength);

        public bool MovementIsInside(BoardMovement movement) => movement.IsInsideBox(BoardLength);


        public MovementAttemptResult GetMovementAttemptResult(BoardMovement movement)
        {
            if (!PositionIsInside(movement.from) || !PositionIsInside(movement.to)) return MovementAttemptResult.Unvalid;

            return UsedForGame.GetMovementAttemptResult(movement, this);
        }


        public IEnumerator<Tile> GetEnumerator()
        {
            foreach (var tile in tiles)
            {
                yield return tile;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        public Board MakeCopy()
        {
            return new Board(BoardLength, Pieces);
        }

    }
}
