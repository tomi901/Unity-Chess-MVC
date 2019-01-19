using UnityEngine;

namespace Chess
{

    public class GameManager : MonoBehaviour
    {

        [SerializeField]
        private ChessBoardUI board;


        private ChessGame game;


        private void Start()
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            game = ChessGame.StartNew();
            board.Model = game.Board;
        }

    }

}