using System;


namespace Chess
{
    public struct BoardMovement
    {

        public BoardVector from, to;

        private ChessPieceType promotion;
        public ChessPieceType Promotion
        {
            get => promotion;
            set => promotion = (value != ChessPieceType.King) ? value : throw KingPromoteArgumentException;
        }

        private static ArgumentException KingPromoteArgumentException => 
            new ArgumentException("Can't promote to king.", nameof(promotion));

        public string PromotionString
        {
            get
            {
                switch (promotion)
                {
                    case ChessPieceType.None:
                        return string.Empty;
                    case ChessPieceType.Pawn:
                        return "p";
                    case ChessPieceType.Rook:
                        return "r";
                    case ChessPieceType.Bishop:
                        return "b";
                    case ChessPieceType.Knight:
                        return "k";
                    case ChessPieceType.Queen:
                        return "q";
                    case ChessPieceType.King:
                        throw KingPromoteArgumentException;
                    default:
                        return "?";
                }
            }
        }


        public BoardMovement(BoardVector from, BoardVector to, ChessPieceType promotion = ChessPieceType.None)
            : this (to, promotion)
        {
            this.from = from;
        }

        public BoardMovement(BoardVector to, ChessPieceType promotion = ChessPieceType.None)
        {
            if (promotion == ChessPieceType.King)
                throw KingPromoteArgumentException;

            this.from = default;
            this.to = to;
            this.promotion = promotion;
        }


        public bool IsInsideBox(BoardVector boxLength) => from.IsInsideBox(boxLength) && to.IsInsideBox(boxLength);


        public override string ToString()
        {
            return from.ToStringCoordinates() + to.ToStringCoordinates() + PromotionString;
        }


        public static explicit operator BoardMovement(BoardVector boardVector)
        {
            return new BoardMovement(boardVector);
        }

        public static BoardMovement operator +(BoardMovement movement, BoardVector add)
        {
            movement.from += add;
            movement.to += add;
            return movement;
        }

    }
}