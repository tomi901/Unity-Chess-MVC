using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public class PieceSelectionPanel : MonoBehaviour
    {

        [SerializeField]
        private PieceButton pieceButtonPrefab = default;

        [SerializeField]
        private Transform buttonsContainer = default;


        private List<PieceButton> buttons = new List<PieceButton>();

        private Action<ChessPieceType> onPieceSelected = null;


        public void Show(IEnumerable<(PieceTeam, ChessPieceType)> pieces, Action<ChessPieceType> onPieceSelected = null)
        {
            gameObject.SetActive(true);

            Clear();
            foreach ((PieceTeam, ChessPieceType) piece in pieces)
            {
                PieceButton newButton = Instantiate(pieceButtonPrefab, buttonsContainer);
                newButton.PieceType = piece.Item2;
                newButton.OnClick += () =>
                {
                    SelectPiece(newButton.PieceType);
                    Close();
                };

                buttons.Add(newButton);
            }

            this.onPieceSelected = onPieceSelected;
        }

        public void Close()
        {
            SelectPiece(ChessPieceType.None);
            gameObject.SetActive(false);
        }

        private void SelectPiece(ChessPieceType pieceType)
        {
            if (onPieceSelected != null)
            {
                onPieceSelected(pieceType);
                onPieceSelected = null;
            }
        }


        protected void Clear()
        {
            buttons.ForEach(b => Destroy(b.gameObject));
            buttons.Clear();
        }

    }
}
