using System;
using System.Linq;


namespace Chess.Player.AI
{
    public class ChessMinMaxAI : ChessAI
    {
        public ChessMinMaxAI(ChessGame game, PieceTeam team, int maxTurnsDepth = 10) : base(game, team)
        {
        }


        protected override void OnTurnStart()
        {
            BoardMovement[] movements = Game.CurrentTurn.AllPossibleMovements.Keys.ToArray();
            DoMovement(movements[RNG.Next(movements.Length)]);
        }


        public int GetPieceValue(Piece piece)
        {
            int absoluteValue;
            switch (piece)
            {
                case PiecePawn _:
                    absoluteValue = 1;
                    break;
                case PieceBishop _:
                case PieceRook _:
                    absoluteValue = 5;
                    break;
                case PieceKnight _:
                    absoluteValue = 10;
                    break;
                case PieceQueen _:
                    absoluteValue = 20;
                    break;
                case PieceKing _:
                    absoluteValue = 1000;
                    break;
                default:
                    throw new NotImplementedException();
            };

            return (piece.Team == Team) ? absoluteValue : -absoluteValue;
        }
    }
}