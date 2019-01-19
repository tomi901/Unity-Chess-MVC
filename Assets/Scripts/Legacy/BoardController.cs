﻿using System.Collections.Generic;

namespace Chess
{

    public class BoardController
    {

        private readonly IBoardView view;
        private readonly IBoard model;

        private readonly List<PieceController> pieces = new List<PieceController>();

        public BoardController(IBoardView view, IBoard model, System.Func<IPieceView> createNewPieceFunc)
        {
            this.view = view;
            this.model = model;

            // Initialization
            view.Size = model.BoardLength;

            foreach (Piece piece in model)
            {
                IPieceView newPieceView = createNewPieceFunc.Invoke();
                newPieceView.Board = view;

                pieces.Add(new PieceController(piece, newPieceView));
            }
        }

    }

}