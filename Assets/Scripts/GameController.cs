using System;
using UnityEngine;

using TMPro;
using DG.Tweening;

namespace Chess
{

    public class GameController : MonoBehaviour
    {

        [SerializeField]
        private ChessBoardUI board = default;

        [Header("UI")]

        [SerializeField]
        private TextMeshProUGUI turnText = default;

        [SerializeField]
        private PieceSelectionPanel pieceSelectionPanel = default;
        public PieceSelectionPanel PieceSelectionPanel => pieceSelectionPanel;


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
            DOTween.Init();
            StartNewGame();
        }

        private void StartNewGame()
        {
            Game = ChessGame.StartNew();
            board.GameController = this;
        }


        private void UpdateTurnInfo()
        {
            turnText.text = $"Turn {game.CurrentTurnNumber}\n" +
                $"Team {game.CurrentTurnTeam}\n" +
                $"Check: {game.CurrentTurnCheck}\n" +
                $"Filtered turns check: {game.CurrentTurn.FilteredNextTurnsCheck}\n"+
                $"Move: {game.CurrentTurn.LastMovement}";
        }

        // Event listeners

        private void OnNextTurn(object sender, EventArgs eventArgs) => UpdateTurnInfo();


        private void OnGameEnded(object sender, EventArgs eventArgs)
        {
            switch (Game.CurrentTurn.FilteredNextTurnsCheck)
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