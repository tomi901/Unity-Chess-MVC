using System;
using System.Collections.Generic;
using System.Linq;


namespace Chess.Player.AI
{
    public class ChessMinMaxAI : ChessAI
    {
        public int MaxTurnsDepth { get; } = 10;


        private Dictionary<Turn, int> cachedScores = new Dictionary<Turn, int>();


        public ChessMinMaxAI(ChessGame game, PieceTeam team, int maxTurnsDepth = 10) : base(game, team)
        {
            MaxTurnsDepth = maxTurnsDepth;
        }


        protected override void OnTurnStart()
        {
            BoardMovement[] movements = Game.CurrentTurn.AllPossibleMovements.Keys.ToArray();
            DoMovement(movements[RNG.Next(movements.Length)]);
        }


        public int StartMinMax(Turn fromTurn, int depth, bool maximizing = true)
        {
            return MinMax(fromTurn, depth, int.MinValue, int.MaxValue, maximizing);
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
        public int MinMax(Turn turn, int depth, int alpha, int beta, bool maximizing)
        {
            if (depth == 0 || !turn.HasMovementsLeft)
                return EvaluateTurn(turn);

            turn.CacheCurrentPossibleMovements();
            if (maximizing)
            {
                int maxEvaluation = int.MinValue;
                foreach (var movementTurn in turn.NextCalculatedTurnsMovements)
                {
                    int evaluation = MinMax(movementTurn.Value, depth - 1, alpha, beta, false);
                    maxEvaluation = Math.Max(maxEvaluation, evaluation);
                    alpha = Math.Max(alpha, evaluation);
                    if (beta <= alpha)
                        break;
                }
                return maxEvaluation;
            }
            else
            {
                int minEvaluation = int.MaxValue;
                foreach (var movementTurn in turn.NextCalculatedTurnsMovements)
                {
                    int evaluation = MinMax(movementTurn.Value, depth - 1, alpha, beta, true);
                    minEvaluation = Math.Min(minEvaluation, evaluation);
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
        public int EvaluateTurn(Turn turn)
        {
            if (cachedScores.TryGetValue(turn, out int score))
            {
                return score;
            }

            // Get the sum of the pieces values
            score = turn.Board.Pieces.Sum(GetPieceValue);
            cachedScores.Add(turn, score);
            return score;
        }

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