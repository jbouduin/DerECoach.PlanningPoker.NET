
namespace DerECoach.PlanningPoker.Core.Domain
{
    public class Participant
    {
        #region private fields ------------------------------------------------        
        #endregion

        #region public properties ---------------------------------------------
        public string ScreenName { get; private set; }
        public string LastEstimation { get; set; }
        public string Uuid { get; private set; }
        public bool IsScrumMaster { get; private set; }
        #endregion

        #region factory methods -----------------------------------------------
        public static Participant CreateParticipant(string screenName, string uuid, bool isScrumMaster = false)
        {
            return new Participant
            {
                ScreenName = screenName,
                Uuid = uuid,
                LastEstimation = "",
                IsScrumMaster = isScrumMaster
            };
        }
        #endregion

    }
}
