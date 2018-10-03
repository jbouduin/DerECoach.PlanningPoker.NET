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
        private readonly IValueResultFactory _valueResultFactory = ValueResultFactoryProvider.GetValueResultFactory();
        private readonly IResultFactory _resultFactory = ResultFactoryProvider.GetResultFactory();
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

        public IValueResult<Game> JoinGame(JoinRequest request, string connectionId)
        {            
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                if (!_games.ContainsKey(request.TeamName))
                    return _valueResultFactory
                        .Failure<Game>(null, string.Format("No game named '{0}' exists", request.TeamName));

                var game = _games[request.TeamName];
                if (game.HasParticipantWithScreenName(request.ScreenName))
                    return _valueResultFactory
                        .Failure<Game>(null, string.Format(
                            "The game '{0}' already has a participant with name '{1}'", 
                            request.TeamName, 
                            request.ScreenName));
                game.AddParticipant(
                    Participant.CreateParticipant(request.ScreenName, request.Uuid, connectionId));
                _connections.Add(connectionId, request.TeamName);

                return _valueResultFactory.Success(game);
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
            
        }

        public async Task<IValueResult<Game>> JoinGameAsync(JoinRequest request, string connectionId)
        {
            return await Task.Run(() =>
            {
                return JoinGame(request, connectionId);
            });
        }

        public IValueResult<Game> RejoinGame(JoinRequest request, string connectionId)
        {            
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                if (!_games.ContainsKey(request.TeamName))
                    return _valueResultFactory
                        .Failure<Game>(null, string.Format("Game named '{0}' has ended", request.TeamName));

                if (_connections.ContainsKey(connectionId))
                    return _valueResultFactory
                        .Failure<Game>(null, "Could not reconnect");

                var participant = _games[request.TeamName].GetParticipantByScreenName(request.ScreenName);
                var reConnectResult = _resultFactory.Failure(
                    string.Format(
                        "No participant with name '{0}' found in game '{0}'",
                        request.ScreenName,
                        request.TeamName));                    ;
                participant.IfNotNull(p => reConnectResult = p.Reconnect(_resultFactory, connectionId));
                _connections.Add(connectionId, request.TeamName);
                return reConnectResult.ConvertToValueResult(r =>
                {
                    if (r.Succeeded)
                    {
                        return _games[request.TeamName];
                    }
                    else
                    {
                        return null;
                    }
                });
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
            
        }

        public async Task<IValueResult<Game>> RejoinGameAsync(JoinRequest request, string connectionId)
        {
            return await Task.Run(() =>
            {
                return RejoinGame(request, connectionId);
            });
        }
        
        public IValueResult<Participant> LeaveGame(LeaveRequest request, string connectionId)
        {
            try
            {
                //readerWriterLock.AcquireReaderLock(10000);
                if (_connections.ContainsKey(connectionId))
                {
                    _connections.Remove(connectionId);
                }
                var game = _games[request.TeamName];                
                return game.RemoveParticipant(_valueResultFactory, request.Uuid);                
            }
            finally
            {
                //readerWriterLock.ReleaseReaderLock();
            }
        }

        public async Task<IValueResult<Participant>> LeaveGameAsync(LeaveRequest request, string connectionId)
        {
            return await Task.Run(() =>
            {
                return LeaveGame(request, connectionId);
            });
        }

        public IValueResult<Game> CreateGame(CreateRequest request, string connectionId)
        {            
            try
            {
                //readerWriterLock.AcquireWriterLock(READ_LOCK_TIMEOUT);
                if (_games.ContainsKey(request.TeamName))
                    return _valueResultFactory
                        .Failure<Game>(null, string.Format("A game named '{0}' already exists", request.TeamName));

                var game = new Game(request.TeamName);
                game.AddParticipant(Participant.CreateParticipant(request.ScrumMaster, request.Uuid, connectionId, true));
                
                _games.Add(request.TeamName, game);
                _connections.Add(connectionId, request.TeamName);
                return _valueResultFactory.Success(game);
            }
            finally
            {
                //readerWriterLock.ReleaseWriterLock();
            }
        }

        public async Task<IValueResult<Game>> CreateGameAsync(CreateRequest request, string connectionId)
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
