
using DerECoach.Common.BaseTypes;
using System.Net;

namespace DerECoach.PlanningPoker.Core.Domain
{
    public class Participant
    {
        #region private fields ------------------------------------------------ 
        private string _connectionId;        
        #endregion

        #region public properties ---------------------------------------------
        public string ScreenName { get; private set; }        
        public string Uuid { get; private set; }
        public bool ScrumMaster { get; private set; }
        public bool Waiting { get; set; }
        public bool IsConnected { get { return _connectionId != null; } }
        #endregion

        #region public methods ------------------------------------------------
        public IResult Reconnect(IResultFactory resultFactory, string connectionId)
        {
            if (connectionId != null)
            {
                _connectionId = connectionId;
                return resultFactory.Success();
            }
            else
            {
                return resultFactory
                    .Failure(string.Format(
                        "The participant with name '{0}' is already connected", 
                        ScreenName));
            }
        }

        public void Disconnect()
        {
            _connectionId = null;
        }

        public bool HasConnectionId(string connectionId)
        {
            return string.Equals(_connectionId, connectionId);
        }
        #endregion

        #region constructor ---------------------------------------------------
        private Participant()
        {
        }
        #endregion

        #region factory methods -----------------------------------------------
        public static Participant CreateParticipant(string screenName, string uuid, string connectionId, bool isScrumMaster = false)
        {
            return new Participant
            {
                ScreenName = screenName,
                Uuid = uuid,
                ScrumMaster = isScrumMaster,
                Waiting = true,
                _connectionId = connectionId
            };
        }
        #endregion

    }
}
