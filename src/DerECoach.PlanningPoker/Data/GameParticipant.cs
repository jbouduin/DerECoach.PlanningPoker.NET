
namespace DerECoach.PlanningPoker.Data
{
    public class GameParticipant
    {
        #region private fields ------------------------------------------------
        private string _lastEstimation;
        #endregion

        #region public properties ---------------------------------------------
        public string ScreenName { get; private set; }
        public string LastEstimation { get; set; }
        #endregion

        #region factory methods -----------------------------------------------
        public static GameParticipant CreateParticipant(string screenName)
        {
            GameParticipant result = new GameParticipant();
            result.ScreenName = screenName;

            return result;
        }
        #endregion

    }
}
