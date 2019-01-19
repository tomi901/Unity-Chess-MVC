using UnityEngine;

namespace Chess.Utils
{
    public static class ChessUnityUtils
    {

        public static Vector2 Vector2Scale(this BoardVector boardVector, Vector2 scale)
        {
            return new Vector2(boardVector.horizontal * scale.x, boardVector.vertical * scale.y);
        }

        public static Vector2 AsVector2(this BoardVector boardVector)
        {
            return new Vector2(boardVector.horizontal, boardVector.vertical);
        }

    }
}