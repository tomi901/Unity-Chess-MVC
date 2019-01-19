using System.Collections;
using UnityEngine;

namespace Chess
{
    [CreateAssetMenu(fileName = ResourceName, menuName = CommonRootMenuName + "Resources", order = -1000)]
    public class ChessResources : ScriptableObject
    {

        public const string CommonRootMenuName = "Chess/";

        #region Instance and Loading

        private const string ResourceName = "Chess Resources";

        private static ChessResources instance;
        public static ChessResources Instance
        {
            get
            {
                if (instance == null) instance = Resources.Load<ChessResources>(ResourceName);
                return instance;
            }
        }

        public static IEnumerator LoadAsync()
        {
            ResourceRequest loadOperation = Resources.LoadAsync<ChessResources>(ResourceName);
            yield return loadOperation;

            instance = (ChessResources)loadOperation.asset;
        }

        #endregion

        [SerializeField]
        private SpriteSet currentSpriteSet;

        public static Sprite GetSprite(PieceTeam team, PieceType type)
        {
            return Instance.currentSpriteSet.GetPiece(team, type);
        }

    }
}
