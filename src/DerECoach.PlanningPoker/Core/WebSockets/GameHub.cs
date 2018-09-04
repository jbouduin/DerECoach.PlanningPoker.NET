using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using DerECoach.PlanningPoker.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.WebSockets
{
    public class GameHub: Hub
    {
        #region game related --------------------------------------------------
        public async Task<Game> Join(JoinRequest request)
        {
            var participant = Participant.CreateParticipant(request.ScreenName, request.Uuid);
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
            var result = await GameService.GetInstance().JoinGameAsync(request.TeamName, participant);
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("joined", participant);
            
            return result;
        }

        public async Task<Game> Create(CreateRequest request)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
            return await GameService.GetInstance().CreateGameASync(request);
        }

        public async Task Leave(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);

            await Clients.Group(teamName).SendAsync("Send", $"{Context.ConnectionId} has left the group {teamName}.");
        }

        #endregion

        #region estimations related -------------------------------------------
        public async Task Estimate(EstimateRequest request)
        {
            var participant = await GameService.GetInstance().EstimateAsync(request);
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("estimated", participant);
        }
        #endregion
    }
}
