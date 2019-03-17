using System;
using System.Linq;


namespace Chess.AI
{
    public class ChessMinMaxAI
    {

        public ChessGame Game { get; }
        public PieceTeam Team { get; }

        private readonly Random rng = new Random();

        public ChessMinMaxAI(ChessGame game, PieceTeam team)
        {
            Game = game;
            Team = team;

            game.OnTurnChange += OnTurnChange;

            TryToDoMovement();
        }

        private void TryToDoMovement()
        {
            if (Game.Ended || Game.CurrentTurnTeam != Team)
                return;

            BoardMovement[] movements = Game.CurrentTurn.AllPossibleMovements.Keys.ToArray();
            Game.TryToDoMovement(movements[rng.Next(movements.Length)]);
        }


        #region Event Listeners

        private void OnTurnChange(object sender, EventArgs args) => TryToDoMovement();

        #endregion

    }
}