using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using DerECoach.PlanningPoker.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace DerECoach.PlanningPoker.Controllers
{
    [Route("api/game")]
    public class GameController : Controller
    {
        //[HttpPost("join")]
        //public Game Join([FromBody] JoinRequest request)
        //{
        //    var result = GameService.GetInstance().GetGame(request.TeamName);
            
        //    var newParticipant = Participant.CreateParticipant(request.ScreenName, request.Uuid);
        //    result.AddParticipant(newParticipant);            
        //    return result;
        //}

        //[HttpPost("create")]
        //public Game Create([FromBody] CreateRequest request)
        //{
        //    return GameService.GetInstance().CreateGame(request);            
        //}
    }
}
