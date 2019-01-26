using System.Collections.Generic;
using UnityEngine;


namespace Chess
{

    public class ChessBoard2D : MonoBehaviour, IBoardView
    {

        [SerializeField]
        private SpriteRenderer tilePrefab = default;

        [SerializeField]
        private float tileSize = 1f;

        [SerializeField]
        private Color oddColor = default, evenColor = default;


        private SpriteRenderer[,] tiles;

        public BoardVector Size
        {
            get => tiles != null ? new BoardVector(tiles.GetLength(0), tiles.GetLength(1)) : default;
            set
            {
                if (tiles != null) return;

                tiles = new SpriteRenderer[value.horizontal, value.vertical];

                float xOffset = value.horizontal * tileSize * -0.5f + (tileSize * 0.5f);
                float yOffset = value.vertical * tileSize * -0.5f + (tileSize * 0.5f);
                for (int y = 0; y < value.vertical; y++)
                {
                    for (int x = 0; x < value.horizontal; x++)
                    {
                        SpriteRenderer newTile = Instantiate(tilePrefab, 
                            new Vector2(xOffset + x * tileSize, yOffset + y * tileSize), 
                            Quaternion.identity, transform);

                        newTile.color = ((x + y) % 2) == 0 ? evenColor : oddColor;
                        tiles[x, y] = newTile;
                    }
                }
            }
        }

        public void HighlightTiles(IEnumerable<BoardVector> tiles)
        {
            
        }


        public Vector2 GetTilePosition(BoardVector position)
        {
            return GetTilePosition(position.horizontal, position.vertical);
        }

        public Vector2 GetTilePosition(int h, int v)
        {
            return tiles[h, v].transform.position;
        }

    }

}