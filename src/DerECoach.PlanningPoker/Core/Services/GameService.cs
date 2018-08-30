using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

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
        private static ReaderWriterLock readerWriterLock = new ReaderWriterLock();
        #endregion

        #region public methods ------------------------------------------------
        public Game GetGame(string teamName)
        {
            try
            {
                readerWriterLock.AcquireReaderLock(READ_LOCK_TIMEOUT);
                return _games[teamName];
            }
            finally
            {
                readerWriterLock.ReleaseReaderLock();
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
                readerWriterLock.AcquireReaderLock(10000);
                result = _games[teamName];
                result.AddParticipant(participant);
            }
            finally
            {
                readerWriterLock.ReleaseReaderLock();
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
            var result = new Game(request.TeamName, Participant.CreateParticipant(request.ScrumMaster, request.Uuid));
            try
            {
                readerWriterLock.AcquireWriterLock(READ_LOCK_TIMEOUT);
                _games.Add(request.TeamName, result);
            }
            finally
            {
                readerWriterLock.ReleaseWriterLock();
            }
            return result;
        }

        public async Task<Game> CreateGameASync(CreateRequest request)
        {
            return await Task.Run(() =>
            {
                return CreateGame(request);
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
