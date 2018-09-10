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
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        #endregion

        #region public properties ---------------------------------------------
        public string TeamName { get; private set; }        
        public List<Participant> Participants { get; } = new List<Participant>();
        public IList<Estimation> Estimations { get; } = new List<Estimation>();
        #endregion

        #region public methods ------------------------------------------------
        public bool HasParticipant(string screenName)
        {
            try
            {
                //readerWriterLock.AcquireReaderLock(READ_LOCK_TIMEOUT);
                return Participants.Any(a => a.ScreenName.ToLower().Equals(screenName.ToLower()));
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
                    Participants.Add(newParticipant);
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

        public Participant GetParticipant(string uuid)
        {
            return Participants.FirstOrDefault(fod => fod.Uuid == uuid);
        }

        public async Task<Participant> GetParticipantAsync(string uuid)
        {
            return await Task.Run(() =>
            {
                return GetParticipant(uuid);
            });
        }

        public Estimation Estimate(string uuid, int index)
        {
            var result = Estimations.FirstOrDefault(fod => fod.Uuid == uuid);
            if (result != null)
            {
                result.Index = index;
            }
            else
            {
                result = new Estimation
                {
                    Uuid = uuid,
                    Index = index
                };
                Estimations.Add(result);
            }
            return result;           
        }

        public async Task<Estimation> EstimateAsync(string uuid, int index)
        {
            return await Task.Run(() =>
            {
                return Estimate(uuid, index);
            });
        }

        public void StartGame()
        {
            Participants.ForEach(fe => fe.Waiting = false);
        }
        #endregion

        #region constructor ---------------------------------------------------
        public Game(string teamName)
        {
            TeamName = teamName;                        
        }
        #endregion 
    }
}
