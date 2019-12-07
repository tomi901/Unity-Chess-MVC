using System;
using UnityEngine;


namespace Chess
{
    public class PlayerController : MonoBehaviour, Player.IPlayer
    {
        public enum Type { Player, MinMax }

        [SerializeField]
        private Type type = default;


        private Player.Player playerModel;

        public PieceTeam Team { get; private set; } = default;

        public event EventHandler<BoardMovement> OnMovementDecided;


        public void Initialize(ChessGame game, PieceTeam team)
        {
            this.Team = team;
            switch (type)
            {
                case Type.Player:
                    break;
                case Type.MinMax:
                    playerModel = new Player.AI.ChessMinMaxAI(game, team);
                    playerModel.OnMovementDecided += OnDecideMovement;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void OnDecideMovement(object sender, BoardMovement movement)
        {
            // Debug.Log($"{sender}: {movement}");
            OnMovementDecided?.Invoke(this, movement);
        }
    }
}