using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.Domain
{
    public class Game
    {
        #region constants -----------------------------------------------------
        private const int READ_LOCK_TIMEOUT = 3000;
        private const int WRITE_LOCK_TIMEOUT = 10000;
        #endregion

        #region private fields ------------------------------------------------        
        //private static ReaderWriterLock readerWriterLock = new ReaderWriterLock();
        #endregion

        #region public properties ---------------------------------------------
        public string TeamName { get; private set; }
        public Participant ScrumMaster { get; private set; }        
        public IList<Participant> AllParticipants { get; } = new List<Participant>();
        #endregion

        #region public methods ------------------------------------------------
        public bool HasParticipant(string screenName)
        {
            try
            {
                //readerWriterLock.AcquireReaderLock(READ_LOCK_TIMEOUT);
                return AllParticipants.Any(a => a.ScreenName.ToLower().Equals(screenName.ToLower()));
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }

        public async Task<bool> HasParticipantASync(string screenName)
        {
            return await Task.Run(() =>
            {
                return HasParticipant(screenName);
            });
        }

        public void AddParticipant(Participant newParticipant)
        {
            if (!HasParticipant(newParticipant.ScreenName))
            {
                try
                {
                    //readerWriterLock.AcquireWriterLock(WRITE_LOCK_TIMEOUT);
                    AllParticipants.Add(newParticipant);
                }
                finally
                {
                    //readerWriterLock.ReleaseWriterLock();
                }
            }
        }
        
        public async Task AddParticipantAsync(Participant newParticipant)
        {
            await Task.Run(() =>
            {
                AddParticipant(newParticipant);
            });
        }
        #endregion

        #region constructor ---------------------------------------------------
        public Game(string teamName, Participant scrumMaster)
        {
            TeamName = teamName;            ;
            ScrumMaster = scrumMaster;
        }
        #endregion 
    }
}
