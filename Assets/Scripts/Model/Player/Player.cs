using System;


namespace Chess.Player
{
    public interface IPlayer
    {
        PieceTeam Team { get; }

        event EventHandler<BoardMovement> OnMovementDecided;
    }

    public class Player : IPlayer
    {
        public ChessGame Game { get; }
        public PieceTeam Team { get; }

        public bool IsInTurn => !Game.Ended && (Game.CurrentTurnTeam == Team);

        protected Random RNG { get; } = new Random();


        public event EventHandler<BoardMovement> OnMovementDecided;


        public Player(ChessGame game, PieceTeam team)
        {
            Game = game;
            Team = team;

            game.OnTurnChange += OnTurnChange;
        }

        private void OnTurnChange(object sender, EventArgs args)
        {
            if (IsInTurn)
                OnTurnStart();
        }

        protected virtual void OnTurnStart() { }


        protected void DoMovement(BoardMovement movement)
        {
            if (!IsInTurn) return;

            OnMovementDecided.Invoke(this, movement);
        }
    }
}
