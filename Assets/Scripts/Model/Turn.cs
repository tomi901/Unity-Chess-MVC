using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Turn
    {

        public int Number { get; private set; } = 1;
        public PieceTeam Team { get; private set; }

        public BoardMovement LastMovement { get; private set; }

        public PieceTeam CheckedTeam { get; private set; }


        private readonly ChessGame game;
        public ChessGame ForGame => game;

        private readonly Board board;
        public Board Board => board;


        private readonly Dictionary<BoardMovement, Turn> allPossibleMovements = new Dictionary<BoardMovement, Turn>();
        private ILookup<BoardVector, BoardVector> possibleMovementsLookup;


        public event EventHandler OnNext = (o, e) => { };


        public Turn(ChessGame forGame, Board board, PieceTeam startingTeam)
        {
            this.game = forGame;

            this.board = board;
            board.UsedInTurn = this;

            this.Team = startingTeam;
        }

        public Turn(ChessGame forGame, Board board, PieceTeam team, int number, BoardMovement movement) 
            : this (forGame, board, team)
        {
            this.Number = number;
            this.LastMovement = movement;
        }


        private static PieceTeam GetNextTeam(PieceTeam currentTeam)
        {
            switch (currentTeam)
            {
                case PieceTeam.White:
                    return PieceTeam.Black;
                default:
                case PieceTeam.Black:
                    return PieceTeam.White;
            }
        }


        public bool CanDoMovement(BoardMovement movement)
        {
            return allPossibleMovements.ContainsKey(movement);
        }

        public IEnumerable<BoardVector> GetAllMovementTargetsFrom(BoardVector position)
        {
            return possibleMovementsLookup[position];
        }


        public void Next(BoardMovement withMovement)
        {
            Number++;
            Team = GetNextTeam(Team);
            LastMovement = withMovement;

            CacheCurrentPossibleMovements();

            OnNext(this, EventArgs.Empty);
        }

        public void CacheCurrentPossibleMovements()
        {
            allPossibleMovements.Clear();
            foreach (BoardMovement movement in board.Pieces.SelectMany(piece => piece.GetAllLegalMovements()))
            {
                allPossibleMovements.Add(movement, null);
                // TODO: Add Turn values when needed and filter the ones that would cause a check for
                // current team
            }

            possibleMovementsLookup = allPossibleMovements.Keys.ToLookup(mov => mov.from, mov => mov.to);
        }


        public Turn MakeCopy()
        {
            return new Turn(game, board.MakeCopy(), Team, Number, LastMovement);
        }

    }
}
