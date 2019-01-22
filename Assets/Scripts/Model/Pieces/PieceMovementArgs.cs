using System;

namespace Chess
{
    public class PieceMovementArgs : EventArgs
    {

        public readonly Tile fromTile, toTile;

        public BoardVector? From => fromTile?.Coordinates;
        public BoardVector? To => toTile?.Coordinates;

        public Board FromBoard => fromTile?.Board;
        public Board ToBoard => toTile?.Board;

        public bool HasMovedToAnotherBoard => FromBoard != ToBoard;


        public PieceMovementArgs(Tile fromTile, Tile toTile)
        {
            this.fromTile = fromTile;
            this.toTile = toTile;
        }

    }
}
