using System;
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


        public Piece MovedPiece { get; private set; }

        public Piece CapturedPiece { get; private set; }
        public bool PieceWasCaptured => CapturedPiece != null;

        // Wrote the flag inverted because it was easier to think the logic according to rules
        private bool NotDrawMovementTurn => MovedPiece is PiecePawn || PieceWasCaptured;
        public bool DrawMovementTurn => !NotDrawMovementTurn;

        public int DrawMovements { get; private set; } = 0;


        public ChessGame ForGame { get; }
        public Board Board { get; }


        private Turn previous = null;
        public Turn Previous { get => previous; private set => previous = value?.AsSimulated; }

        public bool IsSimulated { get; } = false;

        private Turn simulatedTurn = null;
        public Turn AsSimulated => simulatedTurn ?? (simulatedTurn = (IsSimulated ? this : MakeCopy(true)));


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

        
        public bool MovementsAreCached { get; private set; } = false;

        private bool cachedMovementsAreFiltered = false;
        public bool CachedMovementsAreFiltered => MovementsAreCached && cachedMovementsAreFiltered;


        public event EventHandler OnChange = (o, e) => { };


        public Turn(ChessGame forGame, Board board, PieceTeam startingTeam)
        {
            this.ForGame = forGame;

            this.Board = board;
            board.UsedInTurn = this;

            this.Team = startingTeam;
        }

        public Turn(Turn turn, bool simulated = false, BoardMovement? movement = null)
            : this(turn.ForGame, turn.Board.MakeCopy(), turn.Team)
        {
            Number = turn.Number;
            SetTurnMovements(turn);

            if (movement != null)
                Next(movement.Value);

            IsSimulated = simulated;
        }


        // We do this in case we need a list of moved and captured pieces in the future

        public void SetPieceMoved(Piece piece) => MovedPiece = piece;

        public void SetPieceCaptured(Piece piece) => CapturedPiece = piece;


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
            if (IsSimulated)
                throw new Exception("Cannot move with simulated turns.");

            simulatedTurn = null;
            MovedPiece = CapturedPiece = null;

            Board.DoMovement(movement);

            Number++;
            Team = GetNextTeam(Team);
            LastMovement = movement;
            FilteredNextTurnsCheck = PieceTeam.None;

            // Check for draw
            if (ForGame.UseDrawMovements)
            {
                DrawMovements = DrawMovementTurn ? DrawMovements + 1 : 0;

                if (DrawMovements >= ForGame.DrawMovementsQuantity)
                {
                    allPossibleMovements.Clear();
                    removedMovements.Clear();

                    OnChange(this, EventArgs.Empty);
                    return;
                }
            }

            MovementsAreCached = cachedMovementsAreFiltered = false;

            // Cache all the movements, but first try to find if the next turn is in the current cache
            // and copy it's movements
            if (allPossibleMovements.TryGetValue(movement, out Turn next) && next != null)
            {
                SetTurnMovements(next);
            }
            else CacheCurrentPossibleMovements();

            OnChange(this, EventArgs.Empty);
        }


        public void Undo()
        {
            if (Previous != null)
                SetTurn(Previous);
        }

        public void SetTurn(Turn turn)
        {
            SetTurnMovements(turn);

            Number = turn.Number;
            Team = turn.Team;

            simulatedTurn = turn.AsSimulated;

            Board.SetBoard(turn.Board);

            OnChange(this, EventArgs.Empty);
        }


        public void SetTurnMovements(Turn turn)
        {
            LastMovement = turn.LastMovement;

            allPossibleMovements.Clear();
            foreach (var kvp in turn.allPossibleMovements)
            {
                allPossibleMovements.Add(kvp.Key, kvp.Value);
            }

            removedMovements.Clear();
            foreach (var kvp in turn.removedMovements)
            {
                removedMovements.Add(kvp.Key, kvp.Value);
            }

            MovementsAreCached = turn.MovementsAreCached;
            cachedMovementsAreFiltered = turn.cachedMovementsAreFiltered;

            CurrentCheckedTeam = turn.CurrentCheckedTeam;
            FilteredNextTurnsCheck = turn.FilteredNextTurnsCheck;

            MovedPiece = turn.MovedPiece;
            CapturedPiece = turn.CapturedPiece;

            DrawMovements = turn.DrawMovements;

            Previous = turn.Previous;
        }


        public void CacheCurrentPossibleMovements()
        {
            CurrentCheckedTeam = PieceTeam.None;

            allPossibleMovements.Clear();
            foreach (BoardMovement movement in Board.Pieces.SelectMany(piece => piece.GetAllLegalMovements()))
            {
                allPossibleMovements.Add(movement, null);
                PieceTeam checkResult = ForGame.GetCheckResult(movement, Board);
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

            MovementsAreCached = true;
            cachedMovementsAreFiltered = false;
        }


        public void FilterCachedMovements()
        {
            if (!MovementsAreCached)
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
                    Turn simulatedTurn = this.MakeCopy(true, kvp.Key);
                    simulatedTurn.Previous = this;

                    if (!simulatedTurn.IsSimulated || !simulatedTurn.previous.IsSimulated)
                        throw new Exception();

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


        public Turn MakeCopy(bool simulated, BoardMovement? doMovement = null)
        {
            return new Turn(this, simulated, doMovement);
        }

    }
}
