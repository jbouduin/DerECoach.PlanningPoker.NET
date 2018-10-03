using DerECoach.Common.BaseTypes;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public bool HasParticipantWithScreenName(string screenName)
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

        public async Task<bool> HasParticipantWithScreenNameAsync(string screenName)
        {
            return await Task.Run(() =>
            {
                return HasParticipantWithScreenName(screenName);
            });
        }

        public bool HasParticipantWithConnectionId(string connectionId)
        {
            try
            {
                //readerWriterLock.AcquireReaderLock(READ_LOCK_TIMEOUT);
                return Participants.Any(a => a.HasConnectionId(connectionId));
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }

        public async Task<bool> HasParticipantWithConnectionIdAsync(string connectionId)
        {
            return await Task.Run(() =>
            {
                return HasParticipantWithConnectionId(connectionId);
            });
        }

        public void AddParticipant(Participant newParticipant)
        {
            if (!HasParticipantWithScreenName(newParticipant.ScreenName))
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

        public IValueResult<Participant> RemoveParticipant(IValueResultFactory valueResultFactory, string uuid)
        {
            var result = GetParticipant(uuid);
            Participants.Remove(result);
            return valueResultFactory.Success(result);
        }

        public async Task<IValueResult<Participant>> RemoveParticipantAsync(IValueResultFactory valueResultFactory, string uuid)
        {
            return await Task.Run(() =>
            {
                return RemoveParticipant(valueResultFactory, uuid);
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

        public Participant GetParticipantByScreenName(string screenName)
        {
            return Participants.FirstOrDefault(fod => fod.ScreenName == screenName);
        }

        public async Task<Participant> GetParticipantByScreenNameAsync(string screenName)
        {
            return await Task.Run(() =>
            {
                return GetParticipantByScreenName(screenName);
            });
        }

        public Participant GetParticipantByConnectionId(string connectionId)
        {
            return Participants.FirstOrDefault(fod => fod.HasConnectionId(connectionId));
        }

        public async Task<Participant> GetParticipantByConnectionIdAsync(string connectionId)
        {
            return await Task.Run(() =>
            {
                return GetParticipantByConnectionId(connectionId);
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
