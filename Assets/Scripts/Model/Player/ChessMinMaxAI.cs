using System;
using System.Linq;


namespace Chess.Player.AI
{
    public class ChessMinMaxAI : ChessAI
    {
        public ChessMinMaxAI(ChessGame game, PieceTeam team) : base(game, team)
        {
        }


        protected override void OnTurnStart()
        {
            BoardMovement[] movements = Game.CurrentTurn.AllPossibleMovements.Keys.ToArray();
            DoMovement(movements[RNG.Next(movements.Length)]);
        }
    }
}