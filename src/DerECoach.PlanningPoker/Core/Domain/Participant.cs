
namespace DerECoach.PlanningPoker.Core.Domain
{
    public class Participant
    {
        #region private fields ------------------------------------------------        
        #endregion

        #region public properties ---------------------------------------------
        public string ScreenName { get; private set; }        
        public string Uuid { get; private set; }
        public bool ScrumMaster { get; private set; }
        #endregion

        #region constructor ---------------------------------------------------
        private Participant()
        {
        }
        #endregion

        #region factory methods -----------------------------------------------
        public static Participant CreateParticipant(string screenName, string uuid, bool isScrumMaster = false)
        {
            return new Participant
            {
                ScreenName = screenName,
                Uuid = uuid,                
                ScrumMaster = isScrumMaster
            };
        }
        #endregion

    }
}
