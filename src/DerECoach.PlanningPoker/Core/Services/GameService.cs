using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.Services
{
    public class GameService
    {
        #region constants -----------------------------------------------------
        private const int READ_LOCK_TIMEOUT = 3000;
        private const int WRITE_LOCK_TIMEOUT = 10000;
        #endregion

        #region private fields ------------------------------------------------
        private Dictionary<string, Game> _games;
        //private static ReaderWriterLock readerWriterLock = new ReaderWriterLock();
        #endregion

        #region public methods: game related ----------------------------------
        public Game GetGame(string teamName)
        {
            try
            {
                //readerWriterLock.AcquireReaderLock(READ_LOCK_TIMEOUT);
                return _games[teamName];
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }

        public async Task<Game> GetGameAsync(string teamName)
        {
           return await Task.Run(() =>
           {
               return GetGame(teamName);
           });
        }

        public Game JoinGame(string teamName, Participant participant)
        {
            Game result = null;
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                result = _games[teamName];
                result.AddParticipant(participant);
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
            return result;
        }

        public async Task<Game> JoinGameAsync(string teamName, Participant participant)
        {
            return await Task.Run(() =>
            {
                return JoinGame(teamName, participant);
            });
        }

        public Game CreateGame(CreateRequest request)
        {
            var result = new Game(request.TeamName);
            result.AddParticipant(Participant.CreateParticipant(request.ScrumMaster, request.Uuid, true));
            try
            {
                //readerWriterLock.AcquireWriterLock(READ_LOCK_TIMEOUT);
                _games.Add(request.TeamName, result);
                return result;
            }
            finally
            {
                //readerWriterLock.ReleaseWriterLock();
            }
        }

        public async Task<Game> CreateGameASync(CreateRequest request)
        {
            return await Task.Run(() =>
            {
                return CreateGame(request);
            });
        }
        #endregion

        #region methods estimation related ------------------------------------
        public async Task<Participant> Estimate(EstimateRequest request)
        {
            //readerWriterLock.AcquireWriterLock(READ_LOCK_TIMEOUT);
            try
            {
                
                var game = _games[request.TeamName];
                var result = await game.GetParticipantAsync(request.Uuid);
                result.LastEstimation = request.Card;
                return result;
            }
            finally
            {
                //readerWriterLock.ReleaseWriterLock();
            }
            
        }

        public async Task<Participant> EstimateAsync(EstimateRequest request)
        {
            return await Task.Run(() =>
            {
                return Estimate(request);
            });
        }
        #endregion

        #region singleton implementation --------------------------------------
        private static GameService _gameService;
        public static GameService GetInstance()
        {
            return _gameService ?? (_gameService = new GameService());
        }

        private GameService()
        {
            _games = new Dictionary<string, Game>();
        }
        #endregion


    }
}
