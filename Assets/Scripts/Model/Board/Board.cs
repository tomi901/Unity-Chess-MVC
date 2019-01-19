using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{

    public class Board : IBoard
    {

        private readonly Piece[,] pieces;

        public Piece this[BoardVector coords]
        {
            get => this[coords.horizontal, coords.vertical];
            protected set => this[coords.horizontal, coords.vertical] = value;
        }

        public Piece this[int horizontal, int vertical]
        {
            get => pieces[horizontal, vertical];
            protected set
            {
                pieces[horizontal, vertical] = value;

                if (value != null && value.ContainingBoard != this)
                {
                    value.SetBoard(this, new BoardVector(horizontal, vertical));
                    value.OnCoordinatesChanged += OnPieceMove;
                }
            }
        }


        public BoardVector BoardLength => new BoardVector(pieces.GetLength(0), pieces.GetLength(1));


        public Board(BoardVector size)
        {
            pieces = new Piece[size.horizontal, size.vertical];
        }

        public Board(BoardVector size, IEnumerable<IPieceEntry> pieceEntries) : this(size)
        {
            foreach (Piece piece in pieceEntries.SelectMany(entry => entry.GetPieces(this)))
            {
                BoardVector pos = piece.Coordinates;
                if (this[pos] != null)
                {
                    throw new System.InvalidOperationException($"There's overlapping pieces at position '{pos.ToStringCoordinates(true)}'");
                }

                this[pos] = piece;
            }
        }

        private void OnPieceMove(object sender, PieceMovementArgs args)
        {
            Piece movingPiece = this[args.previousPosition];
            this[args.previousPosition] = null;

            this[movingPiece.Coordinates] = movingPiece;
        }


        public bool PositionIsInside(BoardVector position)
        {
            return position.IsInsideBox(pieces.GetLength(0), pieces.GetLength(1));
        }


        public bool MovementIsLegal(Piece movingPiece, BoardVector toPosition)
        {
            if (!PositionIsInside(toPosition)) return false;

            Piece pieceInTile = this[toPosition];
            return pieceInTile == null || pieceInTile.Team != movingPiece.Team;
        }


        public IEnumerator<Piece> GetEnumerator()
        {
            foreach (Piece piece in pieces)
            {
                if (piece != null) yield return piece;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
