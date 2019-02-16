using UnityEngine;
using TMPro;

namespace Chess
{
    public class PieceButton : MonoBehaviour
    {

        [SerializeField]
        private TextMeshProUGUI text = default;

        [SerializeField]
        private UnityEngine.UI.Button.ButtonClickedEvent onClickEvent = default;
        public event UnityEngine.Events.UnityAction OnClick
        {
            add => onClickEvent.AddListener(value);
            remove => onClickEvent.RemoveListener(value);
        }

        private ChessPieceType pieceType = ChessPieceType.None;
        public ChessPieceType PieceType
        {
            get => pieceType;
            set
            {
                if (value == pieceType) return;

                pieceType = value;
                text.text = pieceType.ToString();
            }
        }


        public void OnButtonClick()
        {
            onClickEvent.Invoke();
        }

    }
}