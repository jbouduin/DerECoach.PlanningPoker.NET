using DerECoach.Common.BaseTypes;
using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using System.Collections.Generic;
using System.Net;
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
        private readonly Dictionary<string, Game> _games;
        private readonly Dictionary<string, string> _connections;
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

        public Game JoinGame(JoinRequest request, string connectionId)
        {
            Game result = null;
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                result = _games[request.TeamName];
                result.AddParticipant(Participant.CreateParticipant(request.ScreenName, request.Uuid, connectionId));
                _connections.Add(connectionId, request.TeamName);
                return result;
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
            
        }

        public async Task<Game> JoinGameAsync(JoinRequest request, string connectionId)
        {
            return await Task.Run(() =>
            {
                return JoinGame(request, connectionId);
            });
        }

        public Game RejoinGame(JoinRequest request, string connectionId)
        {
            Game result = null;
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                result = _games[request.TeamName];
                var participant = result.GetParticipantByScreenName(request.ScreenName);
                participant.Reconnect(connectionId);
                _connections.Add(connectionId, request.TeamName);
                return result;
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
            
        }

        public async Task<Game> RejoinGameAsync(JoinRequest request, string connectionId)
        {
            return await Task.Run(() =>
            {
                return RejoinGame(request, connectionId);
            });
        }
        
        public Participant LeaveGame(LeaveRequest request)
        {
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                var game = _games[request.TeamName];
                return game.RemoveParticipant(request.Uuid);
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }

        public async Task<Participant> LeaveGameAsync(LeaveRequest request)
        {
            return await Task.Run(() =>
            {
                return LeaveGame(request);
            });
        }

        public Result<HttpStatusCode, string, Game> CreateGame(CreateRequest request, string connectionId)
        {            
            try
            {
                //readerWriterLock.AcquireWriterLock(READ_LOCK_TIMEOUT);
                if (_games.ContainsKey(request.TeamName))
                    return Result<HttpStatusCode, string, Game>
                        .Failure(null, string.Format("A game named '{0}' already exists", request.TeamName));

                var game = new Game(request.TeamName);
                game.AddParticipant(Participant.CreateParticipant(request.ScrumMaster, request.Uuid, connectionId, true));
                
                _games.Add(request.TeamName, game);
                _connections.Add(connectionId, request.TeamName);
                return Result<HttpStatusCode, string, Game>.Success(game);
            }
            finally
            {
                //readerWriterLock.ReleaseWriterLock();
            }
        }

        public async Task<Result<HttpStatusCode, string, Game>> CreateGameAsync(CreateRequest request, string connectionId)
        {
            return await Task.Run(() =>
            {
                return CreateGame(request, connectionId);
            });
        }

        public void StartGame(string teamName)
        {
            _games[teamName].StartGame();
        }

        public async Task StartGameAsync(string teamName)
        {
            await Task.Run(() =>
            {
                StartGame(teamName);
            });
        }

        public void EndGame(EndRequest request)
        {
            _games.Remove(request.TeamName);
        }

        public async Task EndGameAsync(EndRequest request)
        {
            await Task.Run(() =>
            {
                EndGame(request);
            });
        }
        #endregion

        #region methods estimation related ------------------------------------
        public async Task<Estimation> Estimate(EstimateRequest request)
        {
            //readerWriterLock.AcquireWriterLock(READ_LOCK_TIMEOUT);
            try
            {
                
                var game = _games[request.TeamName];
                var result = await game.EstimateAsync(request.Uuid, request.Index);                
                return result;
            }
            finally
            {
                //readerWriterLock.ReleaseWriterLock();
            }
            
        }

        public async Task<Estimation> EstimateAsync(EstimateRequest request)
        {
            return await Task.Run(() =>
            {
                return Estimate(request);
            });
        }
        #endregion


        #region participant related -------------------------------------------
        public string GetTeamByConnectionId(string connectionId)
        {
            _connections.TryGetValue(connectionId, out string result);
            return result;
        }

        public Participant OnDisconnected(string connectionId)
        {
            _connections.TryGetValue(connectionId, out string team);
            if (team == null)
                return null;

            var result = _games[team].GetParticipantByConnectionId(connectionId);
            result.Disconnect();
            return result;
        }

        public async Task<Participant> OnDisconnectedAsync(string connectionId)
        {
            return await Task.Run(() =>
            {
                return OnDisconnected(connectionId);
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
            _connections = new Dictionary<string, string>();
        }
        #endregion


    }
}
