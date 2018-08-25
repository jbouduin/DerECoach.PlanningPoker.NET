using System.Collections.Generic;
using System.Linq;

namespace DerECoach.PlanningPoker.Data
{
    public class Game
    {
        #region public properties ---------------------------------------------
        public string ScreenName { get; private set; }
        public IList<GameParticipant> AllParticipants { get; } = new List<GameParticipant>();
        #endregion

        #region public methods ------------------------------------------------
        public void AddParticipant(GameParticipant newParticipant)
        {
            if (AllParticipants.Any(a => a.ScreenName.ToLower().Equals(newParticipant.ScreenName.ToLower())))
                return;
            ScreenName = newParticipant.ScreenName;
            AllParticipants.Add(newParticipant);
        }
        #endregion

        #region singleton implementation --------------------------------------
        private static Game _game;
        public static Game GetGame()
        {
            return _game ?? (_game = new Game());
        }

        private Game()
        {
        }
        #endregion 
    }
}
