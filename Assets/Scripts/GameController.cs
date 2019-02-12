using System;
using UnityEngine;
using TMPro;

namespace Chess
{

    public class GameController : MonoBehaviour
    {

        [SerializeField]
        private ChessBoardUI board = default;

        [Header("UI")]

        [SerializeField]
        private TextMeshProUGUI turnText = default;


        private ChessGame game;
        public ChessGame Game
        {
            get => game;
            set
            {
                if (value == game) return;

                game = value;

                game.OnTurnChange += OnNextTurn;
                game.OnGameEnded += OnGameEnded;

                UpdateTurnInfo();
            }
        }


        private void Start()
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            Game = ChessGame.StartNew();
            board.Model = game.Board;
        }


        private void UpdateTurnInfo()
        {
            turnText.text = $"Turn {game.CurrentTurnNumber}\n" +
                $"Team {game.CurrentTurnTeam}\n" +
                $"Check: {game.CurrentTurnCheck}\n" +
                $"Next check: {game.CurrentTurn.NextTurnsCheck}";
        }

        // Event listeners

        private void OnNextTurn(object sender, EventArgs eventArgs)
        {
            if (game.CurrentTurn.IsAnyTeamChecked)
            {
                Debug.Log($"Turn {game.CurrentTurnNumber}: Check.");
            }
            UpdateTurnInfo();
        }


        private void OnGameEnded(object sender, EventArgs eventArgs)
        {
            switch (Game.CurrentTurn.NextTurnsCheck)
            {
                case PieceTeam.None:
                    Debug.Log("Tie.");
                    break;
                case PieceTeam.White:
                    Debug.Log("Black wins!");
                    break;
                case PieceTeam.Black:
                    Debug.Log("White wins!");
                    break;
                default:
                    Debug.LogWarning("Game ended (Unknow case).");
                    break;
            }
        }

    }

}