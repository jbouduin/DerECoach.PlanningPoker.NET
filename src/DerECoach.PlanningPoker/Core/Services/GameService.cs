using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.Services
{
    public class GameService
    {
        #region private fields ------------------------------------------------
        private Dictionary<string, Game> _games;
        #endregion

        #region public methods ------------------------------------------------
        public Game GetGame(string teamName)
        {
            return _games[teamName];
        }

        public Game CreateGame(CreateRequest request)
        {
            var result = new Game(request);
            _games.Add(request.TeamName, result);
            return result;
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
