using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using Chess.Utils;

namespace Chess
{
    public class ChessBoardUI : MonoBehaviour
    {

        [SerializeField]
        private GridLayoutGroup gridLayoutGroup = default;
        public Vector2 TileSize => gridLayoutGroup.cellSize;

        [Header("Tiles")]

        [SerializeField]
        private TileUI tilePrefab = null;

        [SerializeField]
        private Color evenColor = Color.white, oddColor = Color.black;

        [SerializeField]
        private RectTransform tilesContainer = null;

        [Space]

        [SerializeField]
        private Color emptySpaceHighlightColor = new Color(1, 1, 1, 0.5f);
        [SerializeField]
        private Color occupiedSpaceHighlightColor = new Color(1, 1, 1, 0.5f);


        [Header("Pieces")]

        [SerializeField]
        private ChessPieceUI piecePrefab = null;

        [SerializeField]
        private RectTransform piecesContainer = null;
        public RectTransform PiecesContainer => piecesContainer;

        [Space]

        [SerializeField]
        private float moveAnimationDuration = 0.2f;
        public float PieceMoveDuration => moveAnimationDuration;


        private List<ChessPieceUI> pieces = new List<ChessPieceUI>();
        public bool AllPiecesDraggable
        {
            get => pieces.All(p => p.IsDraggable);
            set => pieces.ForEach(p => p.IsDraggable = value);
        }


        private TileUI[,] tiles;


        public TileUI this[BoardVector position] => this[position.horizontal, position.vertical];

        public TileUI this[int h, int v] => tiles[h, v];


        private Board model;
        public Board Model
        {
            get => model;
            set
            {
                model = value;
                Size = model.BoardLength;

                foreach (Piece piece in model.Pieces) AddPiece(piece);
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


        private void AddPiece(Piece piece)
        {
            ChessPieceUI newPiece = Instantiate(piecePrefab, PiecesContainer);
            newPiece.Board = this;
            newPiece.Model = piece;

            newPiece.OnObjectDestroy += () => pieces.Remove(newPiece);

            pieces.Add(newPiece);
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


        public void HighlightTilesFromMovementOrigin(BoardVector origin)
        {
            HighlightTiles(Model.UsedForGame.GetAllMovementsFromTileInCurrentTurn(origin).Select(m => m.to));
        }

        public void HighlightTiles(IEnumerable<BoardVector> tiles)
        {
            ResetHighlightedTiles();

            foreach (BoardVector position in tiles)
            {
                TileUI tile = this[position];
                tile.Highlighted = true;
                tile.HighlightColor = model[position].HasPiece ? occupiedSpaceHighlightColor : emptySpaceHighlightColor;
            }
        }

        public void ResetHighlightedTiles()
        {
            foreach (TileUI tile in tiles) tile.Highlighted = false;
        }

    }
}
