using System;
using UnityEngine;

namespace Chess
{
    public class ChessPiece2D : MonoBehaviour//, IPieceView
    {

        //[SerializeField]
        //private SpriteRenderer spriteRenderer = default;

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


        private BoardVector currentPosition;
        public BoardVector Position
        {
            get => currentPosition;
            set
            {
                if (value == currentPosition) return;

                currentPosition = value;
                UpdatePosition();
            }
        }

        private ChessBoard2D board;
        public IBoardView Board
        {
            get => board;
            set
            {
                try
                {
                    board = (ChessBoard2D)value;
                }
                catch (InvalidCastException)
                {
                    throw new Exception($"{GetType()} can't use a board that is not of type '{typeof(ChessBoard2D)}'");
                }
                UpdatePosition();
            }
        }

        public event EventHandler<PieceSetTargetMovementArgs> OnSetMoveToTile = (o, e) => { };


        private void UpdateSprite()
        {
            //spriteRenderer.sprite = ChessResources.GetSprite(pieceTeam, pieceType);
        }

        private void UpdatePosition()
        {
            if (board == null) return;

            transform.position = board.GetTilePosition(currentPosition);
        }

    }
}
