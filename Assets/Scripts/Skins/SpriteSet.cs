using UnityEngine;

namespace Chess
{
    [CreateAssetMenu(fileName = "Sprite Set", menuName = ChessResources.CommonRootMenuName + "Sprite Set")]
    public class SpriteSet : PieceSet<Sprite, SpriteSet.Team>
    {
        [System.Serializable]
        public class Team : TeamSet<Sprite> { }
    }
}