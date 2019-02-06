
namespace Chess
{
    public enum MovementAttemptResult
    {
        Unvalid = 0,
        Unblocked,
        SameTeam,
        OtherTeam,
    }

    public static class MovementAttemptResults
    {

        public static bool IsUnblockedOrOtherTeam(this MovementAttemptResult attemptResult)
        {
            return attemptResult == MovementAttemptResult.Unblocked || attemptResult == MovementAttemptResult.OtherTeam;
        }

    }
}
