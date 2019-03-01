using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chess
{
    public class Turn
    {

        public int Number { get; private set; } = 1;
        public PieceTeam Team { get; private set; }

        public BoardMovement? LastMovement { get; private set; }

        public PieceTeam CurrentCheckedTeam { get; private set; }

        public PieceTeam FilteredNextTurnsCheck { get; private set; }
        public bool IsAnyFilteredTurnChecked => FilteredNextTurnsCheck != PieceTeam.None;


        private readonly ChessGame game;
        public ChessGame ForGame => game;

        private readonly Board board;
        public Board Board => board;


        private readonly Dictionary<BoardMovement, Turn> allPossibleMovements = new Dictionary<BoardMovement, Turn>();

        private ReadOnlyDictionary<BoardMovement, Turn> allPossibleMovementsRo = null;
        public ReadOnlyDictionary<BoardMovement, Turn> AllPossibleMovements => allPossibleMovementsRo ?? 
            (allPossibleMovementsRo = new ReadOnlyDictionary<BoardMovement, Turn>(allPossibleMovements));


        private readonly Dictionary<BoardMovement, Turn> removedMovements = new Dictionary<BoardMovement, Turn>();

        private ReadOnlyDictionary<BoardMovement, Turn> removedMovementsRo = null;
        public ReadOnlyDictionary<BoardMovement, Turn> RemovedMovements => removedMovementsRo ??
            (removedMovementsRo = new ReadOnlyDictionary<BoardMovement, Turn>(removedMovements));


        public IEnumerable<Turn> AllCalculatedTurns =>
            allPossibleMovements.Select(kvp => kvp.Value).Where(turn => turn != null);

        public IEnumerable<KeyValuePair<BoardMovement, Turn>> NextCalculatedTurnsMovements =>
            AllCalculatedTurns.SelectMany(turn => turn.AllPossibleMovements);


        private bool movementsAreCached = false, cachedMovementsAreFiltered = false;

        public bool MovementsAreCached => movementsAreCached;
        public bool CachedMovementsAreFiltered => movementsAreCached && cachedMovementsAreFiltered;


        public event EventHandler OnNext = (o, e) => { };


        public Turn(ChessGame forGame, Board board, PieceTeam startingTeam)
        {
            this.game = forGame;

            this.board = board;
            board.UsedInTurn = this;

            this.Team = startingTeam;
        }

        public Turn(ChessGame forGame, Board board, PieceTeam team, int number, BoardMovement? movement) 
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


        public bool IsInTurn(Piece piece) => IsInTurn(piece.Team);
        public bool IsInTurn(PieceTeam team) => team == this.Team;


        public bool TeamIsChecked(PieceTeam team) => CurrentCheckedTeam == team;


        public bool CanDoMovement(BoardMovement movement)
        {
            return allPossibleMovements.ContainsKey(movement);
        }


        public IEnumerable<BoardMovement> GetAllMovementsFrom(BoardVector origin)
        {
            return allPossibleMovements.Keys.Where(m => m.from == origin);
        }

        public IEnumerable<BoardMovement> GetAllMovementsTo(BoardVector destination)
        {
            return allPossibleMovements.Keys.Where(m => m.to == destination);
        }

        public IEnumerable<BoardMovement> GetAllMovementsFromAndTo(BoardVector origin, BoardVector destination)
        {
            return allPossibleMovements.Keys.Where(m => m.from == origin && m.to == destination);
        }


        public void Next(BoardMovement movement)
        {
            Board.DoMovement(movement);

            movementsAreCached = cachedMovementsAreFiltered = false;

            Number++;
            Team = GetNextTeam(Team);
            LastMovement = movement;
            FilteredNextTurnsCheck = PieceTeam.None;

            // Cache all the movements, but first try to find if the next turn is in the current cache
            // and copy it's movements
            if (allPossibleMovements.TryGetValue(movement, out Turn next) && next != null)
            {
                allPossibleMovements.Clear();
                removedMovements.Clear();
                foreach (var kvp in next.allPossibleMovements)
                {
                    allPossibleMovements.Add(kvp.Key, kvp.Value);
                }

                CurrentCheckedTeam = next.CurrentCheckedTeam;
                movementsAreCached = true;
            }
            else CacheCurrentPossibleMovements();

            OnNext(this, EventArgs.Empty);
        }


        public void CacheCurrentPossibleMovements()
        {
            CurrentCheckedTeam = PieceTeam.None;

            allPossibleMovements.Clear();
            foreach (BoardMovement movement in board.Pieces.SelectMany(piece => piece.GetAllLegalMovements()))
            {
                allPossibleMovements.Add(movement, null);
                PieceTeam checkResult = game.GetCheckResult(movement, board);
                if (checkResult != PieceTeam.None)
                {
                    if (CurrentCheckedTeam == PieceTeam.None)
                    {
                        CurrentCheckedTeam = checkResult;
                    }
                    else if (checkResult != CurrentCheckedTeam)
                    {
                        // This should never happen (Maybe?)
                    }
                }
            }

            movementsAreCached = true;
            cachedMovementsAreFiltered = false;
        }


        public void FilterCachedMovements()
        {
            if (!movementsAreCached)
            {
                throw new Exception("Cannot filter current possible movements, must cache them first." +
                    $" ({nameof(CacheCurrentPossibleMovements)})");
            }


            // First simulate all the possible turns

            List<KeyValuePair<BoardMovement, Turn>> turnsToModify = new List<KeyValuePair<BoardMovement, Turn>>();

            void SimulateTurns()
            {
                turnsToModify.Clear();
                foreach (KeyValuePair<BoardMovement, Turn> kvp in allPossibleMovements.Where(kvp => kvp.Value == null))
                {
                    Turn simulatedTurn = this.MakeCopy();
                    simulatedTurn.Next(kvp.Key);
                    turnsToModify.Add(new KeyValuePair<BoardMovement, Turn>(kvp.Key, simulatedTurn));
                }
                turnsToModify.ForEach(kvp => allPossibleMovements[kvp.Key] = kvp.Value);
            }

            SimulateTurns();

            // Add extra movements and resimulate them

            foreach (BoardMovement extraMovement in Board.Pieces.SelectMany(piece => piece.GetAllExtraMovements()))
            {
                allPossibleMovements.Add(extraMovement, null);
            }
            SimulateTurns();

            // Then filter all the possible movements

            turnsToModify.Clear();
            turnsToModify.AddRange(allPossibleMovements.Where(kvp => kvp.Value.TeamIsChecked(Team)));

            foreach (KeyValuePair<BoardMovement, Turn> movementTurn in turnsToModify)
            {
                if (FilteredNextTurnsCheck == PieceTeam.None && movementTurn.Value.CurrentCheckedTeam != PieceTeam.None)
                {
                    FilteredNextTurnsCheck = movementTurn.Value.CurrentCheckedTeam;
                }
                allPossibleMovements.Remove(movementTurn.Key);
                removedMovements.Add(movementTurn.Key, movementTurn.Value);
            }

            // Update the possible movements lookup after we filter succesfully
            cachedMovementsAreFiltered = true;
        }


        public Turn MakeCopy()
        {
            return new Turn(game, board.MakeCopy(), Team, Number, LastMovement);
        }

    }
}
