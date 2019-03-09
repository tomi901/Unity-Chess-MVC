using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

using Simius.UI;


namespace Chess
{

    public class ChessPieceUI : MonoBehaviour
    {

        [SerializeField]
        private DraggableUIElement draggable = default;
        public bool IsDraggable { get => draggable.isActiveAndEnabled; set => draggable.enabled = value; }

        [SerializeField]
        private Image image = default;

        private Vector2 currentTileAnchoredPosition;

        private Tween currentTween;
        public bool DoAnimations => true;


        public event System.Action OnObjectDestroy = () => { };


        private Piece model;
        public Piece Model
        {
            get => model;
            set
            {
                if (value == model) return;

                if (model != null)
                {
                    model.OnCoordinatesChanged -= OnMoveToEventListener;
                    model.OnCapture -= OnCaptureEventListener;
                }

                model = value;

                if (model != null)
                {
                    UpdateSprite();
                    UpdatePosition();

                    model.OnCoordinatesChanged += OnMoveToEventListener;
                    model.OnCapture += OnCaptureEventListener;
                }
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
        public Vector2 TargetAnchoredPosition => board.GetAnchoredPositionForTile(Position);

        private void UpdateSprite() => image.sprite = ChessResources.GetSprite(model);

        private void UpdatePosition()
        {
            if (board == null) return;

            currentTileAnchoredPosition = image.rectTransform.anchoredPosition = TargetAnchoredPosition;
        }

        private void OnDestroy()
        {
            CompleteCurrentTween();
            OnObjectDestroy();

            Model = null;
        }

        private void CompleteCurrentTween()
        {
            if (currentTween != null && currentTween.IsActive())
                currentTween.Kill(true);
        }


        #region Model Listeners

        private void OnMoveToEventListener(object sender, PieceMovementArgs args)
        {
            currentTileAnchoredPosition = TargetAnchoredPosition;

            if (DoAnimations)
            {
                Board.AllPiecesDraggable = false;

                CompleteCurrentTween();
                currentTween = image.rectTransform.DOAnchorPos(currentTileAnchoredPosition, board.PieceMoveDuration);
                currentTween.SetRecyclable(true);
                currentTween.OnComplete(() => Board.AllPiecesDraggable = true);
            }
            else image.rectTransform.anchoredPosition = currentTileAnchoredPosition;
        }

        private void OnCaptureEventListener(object sender, System.EventArgs args)
        {
            Destroy(gameObject);
        }

        #endregion


        #region Draggable Events

        public void OnDragBegin()
        {
            board.HighlightTilesFromMovementOrigin(Position);
        }

        public void OnDragEnd()
        {
            BoardVector targetPosition = Board.GetTilePositionFromAnchoredPos(image.rectTransform.anchoredPosition);
            image.rectTransform.anchoredPosition = currentTileAnchoredPosition;
            Board.TryToDoMovement(Position, targetPosition);

            Board.ResetHighlightedTiles();
        }

        #endregion

    }
}