using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public class Turn
    {

        private int number = 1;
        public int Number => number;

        private PieceTeam team;
        public PieceTeam Team => team;

        private BoardMovement lastMovement;


        private readonly ChessGame game;
        public ChessGame ForGame => game;

        private readonly Board board;
        public Board Board => board;


        private readonly HashSet<BoardMovement> allPossiblelMovementsSet = new HashSet<BoardMovement>();
        private ILookup<BoardVector, BoardVector> possibleMovementsLookup;


        public Turn(ChessGame forGame, Board board, PieceTeam startingTeam)
        {
            this.game = forGame;

            this.board = board;
            board.UsedInTurn = this;

            this.team = startingTeam;
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
            return allPossiblelMovementsSet.Contains(movement);
        }

        public IEnumerable<BoardVector> GetAllMovementTargetsFrom(BoardVector position)
        {
            return possibleMovementsLookup[position];
        }


        public void Next()
        {
            number++;
            team = GetNextTeam(team);

            CacheCurrentPossibleMovements();
        }

        public void CacheCurrentPossibleMovements()
        {
            allPossiblelMovementsSet.Clear();
            foreach (BoardMovement movement in board.Pieces.SelectMany(piece => piece.GetAllLegalMovements()))
            {
                allPossiblelMovementsSet.Add(movement);
            }

            possibleMovementsLookup = allPossiblelMovementsSet.ToLookup(mov => mov.from, mov => mov.to);
        }

    }
}
