
namespace DerECoach.PlanningPoker.Core.Domain
{
    public class Participant
    {
        #region private fields ------------------------------------------------
        private string _lastEstimation;
        #endregion

        #region public properties ---------------------------------------------
        public string ScreenName { get; private set; }
        public string LastEstimation { get; set; }
        #endregion

        #region factory methods -----------------------------------------------
        public static Participant CreateParticipant(string screenName)
        {
            Participant result = new Participant();
            result.ScreenName = screenName;

            return result;
        }
        #endregion

    }
}
