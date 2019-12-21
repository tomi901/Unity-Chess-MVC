using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Chess.Player.AI
{
    public class ChessMinMaxAI : ChessAI
    {
        private delegate void MinMaxCalculation(int evaluation, ref int maxCalculation);


        public int MaxTurnsDepth { get; } = 10;
        public float MaxCalculationTime { get; } = 5;


        private readonly List<BoardMovement> chosenMovements = new List<BoardMovement>();


        public ChessMinMaxAI(ChessGame game, PieceTeam team, int maxTurnsDepth = 3, float maxCalculationTime = 5f)
            : base(game, team)
        {
            MaxTurnsDepth = maxTurnsDepth;
            MaxCalculationTime = maxCalculationTime;
        }


        protected override async void OnTurnStart()
        {
            Task timeout = Task.Delay(TimeSpan.FromSeconds(MaxCalculationTime));
            // Asynchronously calculate the movement, with a timeout
            Task completedTask = await Task.WhenAny(Task.Run(() => StartMinMax(CurrentTurn, MaxTurnsDepth)), timeout);

            if (chosenMovements.Count <= 0)
            {
                chosenMovements.AddRange(CurrentTurn.AllPossibleMovements.Keys);
            }
            DoMovement(chosenMovements[RNG.Next(chosenMovements.Count)]);
        }


        public int StartMinMax(Turn fromTurn, int depth, bool maximizing = true)
        {
            chosenMovements.Clear();
            return MinMax(fromTurn, depth, int.MinValue, int.MaxValue, maximizing, chosenMovements);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="maximizing"></param>
        /// <returns></returns>
        public int MinMax(Turn turn, int depth, int alpha, int beta, bool maximizing,
            List<BoardMovement> appendMovements = null)
        {
            if (depth == 0 || !turn.HasMovementsLeft)
                return EvaluateTurn(turn);

            turn.CacheCurrentPossibleMovements();
            turn.FilterCachedMovements();

            /*
            MinMaxCalculation calculation = maximizing ? (MinMaxCalculation)
            ((int evaluation, ref int maxEvaluation) =>
            {
                maxEvaluation = Math.Max(maxEvaluation, evaluation);
                alpha = Math.Max(alpha, evaluation);
            }) :
            ((int evaluation, ref int minEvaluation) =>
            {
                minEvaluation = Math.Min(minEvaluation, evaluation);
                beta = Math.Min(beta, evaluation);
            });
            */


            if (maximizing)
            {
                int maxEvaluation = int.MinValue;
                foreach (var movementTurn in turn.AllPossibleMovements)
                {
                    int evaluation = MinMax(movementTurn.Value, depth - 1, alpha, beta, false);

                    if (evaluation >= maxEvaluation)
                    {
                        maxEvaluation = evaluation;
                        
                        if (appendMovements != null)
                        {
                            if (evaluation > maxEvaluation)
                                chosenMovements.Clear();

                            chosenMovements.Add(movementTurn.Key);
                        }
                    }
                    alpha = Math.Max(alpha, evaluation);
                    if (beta <= alpha)
                        break;
                }
                return maxEvaluation;
            }
            else
            {
                int minEvaluation = int.MaxValue;
                foreach (var movementTurn in turn.AllPossibleMovements)
                {
                    int evaluation = MinMax(movementTurn.Value, depth - 1, alpha, beta, true);

                    if (evaluation <= minEvaluation)
                    {
                        minEvaluation = evaluation;

                        if (appendMovements != null)
                        {
                            if (evaluation < minEvaluation)
                                chosenMovements.Clear();

                            chosenMovements.Add(movementTurn.Key);
                        }
                    }
                    beta = Math.Min(beta, evaluation);
                    if (beta <= alpha)
                        break;
                }
                return minEvaluation;
            }
        }


        /// <summary>
        /// Calculates the min-max score for this AI for the given turn.
        /// </summary>
        /// <param name="turn"></param>
        /// <returns></returns>
        public int EvaluateTurn(Turn turn) => turn.Board.Pieces.Sum(GetPieceValue); // Get the sum of the pieces values

        public int GetPieceValue(Piece piece)
        {
            int absoluteValue;
            switch (piece)
            {
                case PiecePawn _:
                    absoluteValue = 1;
                    break;
                case PieceBishop _:
                case PieceRook _:
                    absoluteValue = 5;
                    break;
                case PieceKnight _:
                    absoluteValue = 10;
                    break;
                case PieceQueen _:
                    absoluteValue = 20;
                    break;
                case PieceKing _:
                    absoluteValue = 1000;
                    break;
                default:
                    throw new NotImplementedException();
            };

            return (piece.Team == Team) ? absoluteValue : -absoluteValue;
        }
    }
}