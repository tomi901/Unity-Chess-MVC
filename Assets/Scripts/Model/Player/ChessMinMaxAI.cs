using System;
using System.Linq;


namespace Chess.AI
{
    public class ChessMinMaxAI : Player.Player
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