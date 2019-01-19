using UnityEngine;
using UnityEngine.UI;

using Chess.Utils;

namespace Chess
{
    public class ChessBoardUI : MonoBehaviour
    {

        [SerializeField]
        private GridLayoutGroup gridLayoutGroup;
        public Vector2 TileSize => gridLayoutGroup.cellSize;

        [Space]

        [SerializeField]
        private TileUI tilePrefab = null;

        [SerializeField]
        private Color evenColor = Color.white, oddColor = Color.black;

        [SerializeField]
        private RectTransform tilesContainer = null;

        [Space]

        [SerializeField]
        private ChessPieceUI piecePrefab = null;

        [SerializeField]
        private RectTransform piecesContainer = null;
        public RectTransform PiecesContainer => piecesContainer;

        private TileUI[,] tiles;


        public TileUI this[BoardVector position] => this[position.horizontal, position.vertical];

        public TileUI this[int h, int v] => tiles[h, v];


        private IBoard model;
        public IBoard Model
        {
            get => model;
            set
            {
                model = value;
                Size = model.BoardLength;

                foreach (Piece piece in model)
                {
                    ChessPieceUI newPiece = Instantiate(piecePrefab, PiecesContainer);
                    newPiece.Board = this;
                    newPiece.Model = piece;
                }
            }
        }


        public BoardVector Size
        {
            get => new BoardVector(tiles.GetLength(0), tiles.GetLength(1));
            set
            {
                if (tiles != null)
                {
                    foreach (TileUI tile in tiles) Destroy(tile.gameObject);
                }

                int hLength = value.horizontal, vLength = value.vertical;
                tiles = new TileUI[hLength, vLength];

                for (int v = 0; v < vLength; v++)
                {
                    for (int h = 0; h < hLength; h++)
                    {
                        TileUI newTile = Instantiate(tilePrefab, tilesContainer);

                        newTile.MainColor = ((h + v) % 2) == 0 ? evenColor : oddColor;
                        newTile.Highlighted = false;
                        newTile.name = $"Tile {new BoardVector(h, v).ToStringCoordinates(true)}";

                        tiles[h, v] = newTile;
                    }
                }
            }
        }

        public bool CoordinateIsInBoard(BoardVector position)
        {
            return position.IsInsideBox(tiles.GetLength(0), tiles.GetLength(1));
        }


        public Vector2 GetAnchoredPositionForTile(BoardVector position)
        {
            Vector2 tileSize = TileSize;
            Vector2 offset = (-Size.Vector2Scale(tileSize) + tileSize) * 0.5f;

            return offset + position.Vector2Scale(tileSize);
        }

        public BoardVector GetTilePositionFromAnchoredPos(Vector2 position)
        {
            Vector2 tileSize = TileSize;
            Vector2 offset = Size.Vector2Scale(tileSize) * -0.5f;
            position -= offset;

            return new BoardVector((int)(position.x / tileSize.x), (int)(position.y / tileSize.y));
        }


        public void HighlightTiles(System.Collections.Generic.IEnumerable<BoardVector> tiles)
        {
            ResetHighlightedTiles();

            foreach (BoardVector tile in tiles)
            {
                this[tile].Highlighted = true;
            }
        }

        public void ResetHighlightedTiles()
        {
            foreach (TileUI tile in tiles) tile.Highlighted = false;
        }

    }
}
