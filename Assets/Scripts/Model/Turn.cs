﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private ReadOnlyDictionary<BoardMovement, Turn> allPossibleMovementsRo = null;
        public ReadOnlyDictionary<BoardMovement, Turn> AllPossibleMovements => allPossibleMovementsRo ?? 
            (allPossibleMovementsRo = new ReadOnlyDictionary<BoardMovement, Turn>(allPossibleMovements));


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


        public bool IsInTurn(Piece piece)
        {
            return IsInTurn(piece.Team);
        }

        public bool IsInTurn(PieceTeam team)
        {
            return team == this.Team;
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
            movementsAreCached = cachedMovementsAreFiltered = false;

            Number++;
            Team = GetNextTeam(Team);
            LastMovement = withMovement;

            // Cache all the movements, but first try to find if the next turn is in the current cache
            // and copy it's movements
            if (allPossibleMovements.TryGetValue(withMovement, out Turn next) && next != null)
            {
                allPossibleMovements.Clear();
                foreach (var kvp in next.allPossibleMovements)
                {
                    allPossibleMovements.Add(kvp.Key, kvp.Value);
                }

                CheckedTeam = next.CheckedTeam;
                movementsAreCached = true;
            }
            else CacheCurrentPossibleMovements();

            OnNext(this, EventArgs.Empty);
        }


        public void CacheCurrentPossibleMovements()
        {
            CheckedTeam = PieceTeam.Unknown;

            allPossibleMovements.Clear();
            foreach (BoardMovement movement in board.Pieces.SelectMany(piece => piece.GetAllLegalMovements()))
            {
                allPossibleMovements.Add(movement, null);
                PieceTeam checkResult = game.GetCheckResult(movement, board);
                if (checkResult != PieceTeam.Unknown)
                {
                    if (CheckedTeam == PieceTeam.Unknown)
                    {
                        CheckedTeam = checkResult;
                    }
                    else if (checkResult != CheckedTeam)
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
            foreach (KeyValuePair<BoardMovement, Turn> kvp in allPossibleMovements.Where(kvp => kvp.Value == null))
            {
                Turn simulatedTurn = this.MakeCopy();
                simulatedTurn.Next(kvp.Key);
                turnsToModify.Add(new KeyValuePair<BoardMovement, Turn>(kvp.Key, simulatedTurn));
            }
            turnsToModify.ForEach(kvp => allPossibleMovements[kvp.Key] = kvp.Value);


            // Then filter all the possible movements

            List<BoardMovement> removeMovements = new List<BoardMovement>(allPossibleMovements
                .Where(kvp => kvp.Value.CheckedTeam == Team).Select(kvp => kvp.Key));

            removeMovements.ForEach(movement => allPossibleMovements.Remove(movement));

            // Update the possible movements lookup after we filter succesfully
            possibleMovementsLookup = allPossibleMovements.Keys.ToLookup(mov => mov.from, mov => mov.to);
            cachedMovementsAreFiltered = true;
        }


        public Turn MakeCopy()
        {
            return new Turn(game, board.MakeCopy(), Team, Number, LastMovement);
        }

    }
}
