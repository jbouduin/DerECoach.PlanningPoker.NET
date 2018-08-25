using DerECoach.PlanningPoker.Core.Requests;
using System.Collections.Generic;
using System.Linq;

namespace DerECoach.PlanningPoker.Core.Domain
{
    public class Game
    {
        #region public properties ---------------------------------------------
        public string TeamName { get; private set; }
        public Participant ScrumMaster { get; private set; }        
        public IList<Participant> AllParticipants { get; } = new List<Participant>();
        #endregion

        #region public methods ------------------------------------------------
        public bool HasParticipant(string screenName)
        {
            return AllParticipants.Any(a => a.ScreenName.ToLower().Equals(screenName.ToLower()));
        }

        public void AddParticipant(Participant newParticipant)
        {
            if (HasParticipant(newParticipant.ScreenName))
                return;        
            AllParticipants.Add(newParticipant);
        }
        #endregion

        #region constructor ---------------------------------------------------
        public Game(CreateRequest request)
        {
            TeamName = request.TeamName;
            ScrumMaster = Participant.CreateParticipant(request.ScrumMaster);
            AddParticipant(Participant.CreateParticipant("John"));
            AddParticipant(Participant.CreateParticipant("Mary"));
        }
        #endregion 
    }
}
