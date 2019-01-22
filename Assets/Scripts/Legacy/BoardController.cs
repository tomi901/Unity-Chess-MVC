using System.Collections.Generic;

namespace Chess
{

    public class BoardController
    {

        private readonly IBoardView view;
        private readonly Board model;

        private readonly List<PieceController> pieces = new List<PieceController>();

        public BoardController(IBoardView view, Board model, System.Func<IPieceView> createNewPieceFunc)
        {
            this.view = view;
            this.model = model;

            // Initialization
            view.Size = model.BoardLength;

            foreach (Piece piece in model.Pieces)
            {
                IPieceView newPieceView = createNewPieceFunc.Invoke();
                newPieceView.Board = view;

                pieces.Add(new PieceController(piece, newPieceView));
            }
        }

    }

}