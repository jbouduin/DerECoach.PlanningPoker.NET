
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
        public string Uuid { get; private set; }
        #endregion

        #region factory methods -----------------------------------------------
        public static Participant CreateParticipant(string screenName, string uuid)
        {
            return new Participant
            {
                ScreenName = screenName,
                Uuid = uuid
            };
        }
        #endregion

    }
}
