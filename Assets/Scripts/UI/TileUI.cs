using UnityEngine;
using UnityEngine.UI;

namespace Chess
{
    public class TileUI : MonoBehaviour
    {

        [SerializeField]
        private Image mainGraphic = null;

        public Color MainColor { get => mainGraphic.color; set => mainGraphic.color = value; }

        [SerializeField]
        private Graphic highlightGraphic = null;


        public Color HighlightColor { get => highlightGraphic.color; set => highlightGraphic.color = value; }

        public bool Highlighted
        {
            get => highlightGraphic.gameObject.activeSelf;
            set => highlightGraphic.gameObject.SetActive(value);
        }

    }
}