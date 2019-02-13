using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;


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


        public event System.Action OnObjectDestroy = () => { };


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
        public Vector2 TargetPosition => board.GetAnchoredPositionForTile(Position);

        private PieceTeam pieceTeam = PieceTeam.None;
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

            currentTileAnchoredPosition = image.rectTransform.anchoredPosition = TargetPosition;
        }


        public void SetMovementTarget(BoardVector position)
        {
            model.TryToMoveTo(position);
        }


        private void OnDestroy()
        {
            OnObjectDestroy();
        }


        // Model Listeners

        private void OnMoveToEventListener(object sender, PieceMovementArgs args)
        {
            currentTileAnchoredPosition = TargetPosition;

            Board.AllPiecesDraggable = false;
            Tweener tween = image.rectTransform.DOAnchorPos(currentTileAnchoredPosition, board.PieceMoveDuration);
            tween.SetRecyclable(true);
            tween.OnComplete(() => Board.AllPiecesDraggable = true);
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