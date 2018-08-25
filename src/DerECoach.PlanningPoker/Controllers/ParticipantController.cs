using DerECoach.PlanningPoker.Data;
using Microsoft.AspNetCore.Mvc;



namespace DerECoach.PlanningPoker.Controllers
{
    [Route("api/participant")]
    public class ParticipantController : Controller
    {
        [HttpPost("join")]
        public Game JoinTeam([FromBody] JoinRequest request)
        {
            var newParticipant = GameParticipant.CreateParticipant(request.ScreenName);
            var result = Game.GetGame();
            result.AddParticipant(newParticipant);
            return result;
        }
    }
}
