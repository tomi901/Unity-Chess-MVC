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
        private GameObject highlightGraphic = null;

        public bool Highlighted { get => highlightGraphic.activeSelf; set => highlightGraphic.SetActive(value); }

    }
}