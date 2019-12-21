using System;
using System.Linq;
using UnityEngine;

using TMPro;
using DG.Tweening;

namespace Chess
{

    public class GameController : MonoBehaviour
    {
        private enum InfoLevel { Basic, Debug }

        [SerializeField]
        private ChessBoardUI board = default;


        [Header("Players")]

        [SerializeField]
        private PlayerController whitePlayer = default;

        [SerializeField]
        private PlayerController blackPlayer = default;


        [Header("UI")]

        [SerializeField]
        private TextMeshProUGUI infoText = default;

        [SerializeField]
        private InfoLevel infoLevel = default;

        [Space]

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

            InitializePlayer(whitePlayer, PieceTeam.White);
            InitializePlayer(blackPlayer, PieceTeam.Black);
        }

        private void InitializePlayer(PlayerController player, PieceTeam team)
        {
            player.Initialize(Game, team);
            Game.ListenToPlayer(player);
        }


        private void UpdateTurnInfo()
        {
            System.Collections.Generic.IEnumerable<string> Info()
            {
                switch (infoLevel)
                {
                    case InfoLevel.Basic:
                        yield return $"Turn {game.CurrentTurnNumber}";
                        yield return "Last Movement: " + (game.CurrentTurn.Previous != null ?
                            game.CurrentTurn.Previous.LastMovement.ToString()
                            : string.Empty);
                        yield return $"Team {game.CurrentTurnTeam} Turn";

                        if (game.Ended)
                        {
                            yield return null;
                            yield return game.CurrentWinner != PieceTeam.None ?
                                $"<size=200%>{game.CurrentWinner} Wins!</size>" :
                                $"<size=200%>Stalemate!</size>";
                        }
                        else if (game.NextTurnsCheck != PieceTeam.None)
                        {
                            yield return null;
                            yield return $"<size=150%>Check!</size>";
                        }

                        break;
                    case InfoLevel.Debug:
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
                        break;
                }

            }

            infoText.text = string.Join("\n", Info());
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