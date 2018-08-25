using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DerECoach.PlanningPoker.Controllers
{
    [Route("api/game")]
    public class GameController : Controller
    {
        [HttpPost("join")]
        public Game Join([FromBody] JoinRequest request)
        {
            var newParticipant = Participant.CreateParticipant(request.ScreenName);
            var result = Game.GetGame();
            result.AddParticipant(newParticipant);
            return result;
        }

        [HttpPost("create")]
        public Game Create([FromBody] CreateRequest request)
        {
            throw new NotImplementedException();
            //var newParticipant = Participant.CreateParticipant(request.ScreenName);
            //var result = Game.GetGame();
            //result.AddParticipant(newParticipant);
            //return result;
        }
    }
}
