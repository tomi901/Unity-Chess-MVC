﻿using System;
using System.Linq;
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

                game.OnTurnChange += OnTurnChangeListener;
                game.OnGameEnded += OnGameEndedListener;

                UpdateTurnInfo();
            }
        }

        public bool CanUndoTurn => Game.CanUndoTurn;


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
            System.Collections.Generic.IEnumerable<string> Info()
            {
                yield return $"Turn {game.CurrentTurnNumber}";
                yield return $"Team {game.CurrentTurnTeam}";
                yield return $"Check: {game.CurrentTurnCheck}";
                yield return $"Filtered turns check: {game.CurrentTurn.FilteredNextTurnsCheck}";

                yield return null;

                yield return $"Move: {game.CurrentTurn.LastMovement}";
                yield return $"Moved piece: {game.CurrentTurn.MovedPiece}";
                yield return $"Captured piece: {game.CurrentTurn.CapturedPiece}";
                yield return $"Draw movements: {game.CurrentTurnDrawMovements}";

                yield return null;

                yield return "Previous: " + (game.CurrentTurn.Previous != null ? 
                    $"{game.CurrentTurn.Previous.Number} ({game.CurrentTurn.Previous.LastMovement})"
                    : string.Empty);
            }

            turnText.text = string.Join("\n", Info());
        }


        public void UndoTurn()
        {
            if (CanUndoTurn)
            {
                Debug.Log($"Undoing movement {Game.CurrentTurn.LastMovement}...", this);
                Game.UndoTurn();
            }
        }


        #region Event listeners

        private void OnTurnChangeListener(object sender, EventArgs eventArgs) => UpdateTurnInfo();


        private void OnGameEndedListener(object sender, EventArgs eventArgs)
        {
            Debug.Log(Game.CurrentWinner != PieceTeam.None ? $"{Game.CurrentWinner} team wins!" : "Tie.", this);
        }

        #endregion

    }

}