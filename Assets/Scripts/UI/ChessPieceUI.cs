﻿using UnityEngine;
using UnityEngine.UI;


namespace Chess
{

    public class ChessPieceUI : MonoBehaviour
    {

        [SerializeField]
        private Image image = default;

        private Vector2 currentTileAnchoredPosition;


        private Piece model;
        public Piece Model
        {
            get => model;
            set
            {
                model = value;

                Team = value.Team;

                UpdatePosition();

                model.OnCoordinatesChanged += OnMoveToEventListener;
                model.OnCapture += OnCaptureEventListener;
            }
        }

        private ChessBoardUI board;
        public ChessBoardUI Board
        {
            get => board;
            set
            {
                if (value == board) return;

                board = value;
                image.rectTransform.SetParent(board.PiecesContainer);
            }
        }

        public BoardVector Position => model.Coordinates;

        /*
        private PieceType pieceType = PieceType.Unknown;
        public PieceType Type
        {
            get => pieceType;
            set
            {
                if (value == pieceType) return;

                pieceType = value;
                UpdateSprite();
            }
        }
        */

        private PieceTeam pieceTeam = PieceTeam.Unknown;
        public PieceTeam Team
        {
            get => pieceTeam;
            set
            {
                if (value == pieceTeam) return;

                pieceTeam = value;
                UpdateSprite();
            }
        }

        private void UpdateSprite()
        {
            image.sprite = ChessResources.GetSprite(model);
        }

        private void UpdatePosition()
        {
            if (board == null) return;

            currentTileAnchoredPosition = board.GetAnchoredPositionForTile(Position);
            image.rectTransform.anchoredPosition = currentTileAnchoredPosition;
        }


        public void SetMovementTarget(BoardVector position)
        {
            model.TryToMoveTo(position);
        }


        // Model Listeners

        private void OnMoveToEventListener(object sender, PieceMovementArgs args)
        {
            UpdatePosition();
            // TODO: Animation
        }

        private void OnCaptureEventListener(object sender, System.EventArgs args)
        {
            Destroy(gameObject);
        }


        // Draggable events

        public void OnDragBegin()
        {
            board.HighlightTiles(board.Model.UsedForGame.GetAllMovementsForTileInCurrentTurn(Position));
        }

        public void OnDragEnd()
        {
            Vector2 targetPosition = image.rectTransform.anchoredPosition;
            image.rectTransform.anchoredPosition = currentTileAnchoredPosition;
            SetMovementTarget(board.GetTilePositionFromAnchoredPos(targetPosition));

            board.ResetHighlightedTiles();
        }

    }
}