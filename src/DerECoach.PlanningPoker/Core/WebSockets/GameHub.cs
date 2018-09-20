using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using DerECoach.PlanningPoker.Core.Responses;
using DerECoach.PlanningPoker.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.WebSockets
{
    public class GameHub: Hub
    {
        #region game related --------------------------------------------------
        public async Task<JoinResponse> Join(JoinRequest request)
        {
            var participant = Participant.CreateParticipant(request.ScreenName, request.Uuid);
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
            var result = new JoinResponse();
            result.Game = await GameService.GetInstance().JoinGameAsync(request.TeamName, participant);
            result.Cards = GetCards();
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("joined", participant);
            
            return result;
        }

        public async Task<CreateResponse> Create(CreateRequest request)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
            var result = new CreateResponse();
            result.Game = await GameService.GetInstance().CreateGameASync(request);
            result.Cards = GetCards();
            return result;
        }

        public async Task Leave(LeaveRequest leaveRequest)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, leaveRequest.TeamName);
            var participant = GameService.GetInstance().LeaveGame(leaveRequest);
            await Clients.Group(leaveRequest.TeamName).SendAsync("left", participant);
        }

        public async Task Start(string teamName)
        {
            await GameService.GetInstance().StartGameAsync(teamName);
            await Clients.GroupExcept(teamName, Context.ConnectionId).SendAsync("started");
        }

        public async Task End(EndRequest request)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, request.TeamName);
            GameService.GetInstance().EndGame(request);
            await Clients.Group(request.TeamName).SendAsync("ended");            
        }
        #endregion

        #region estimations related -------------------------------------------
        public async Task Estimate(EstimateRequest request)
        {            
            var estimation = await GameService.GetInstance().EstimateAsync(request);            
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("estimated", estimation);
        }
        #endregion

        #region helpers -------------------------------------------------------
        private IList<Card> GetCards()
        {
            var result = new List<Card>
            {
                new Card()
                {
                    Index = 0,
                    Label = "0"
                },
                new Card()
                {
                    Index = 1,
                    Label = "½"
                },
                new Card()
                {
                    Index = 2,
                    Label = "1"
                },
                new Card()
                {
                    Index = 3,
                    Label = "2"
                },
                new Card()
                {
                    Index = 4,
                    Label = "3"
                },
                new Card()
                {
                    Index = 5,
                    Label = "5"
                },
                new Card()
                {
                    Index = 6,
                    Label = "8"
                },
                new Card()
                {
                    Index = 7,
                    Label = "13"
                },
                new Card()
                {
                    Index = 8,
                    Label = "20"
                },
                new Card()
                {
                    Index = 9,
                    Label = "40"
                },
                new Card()
                {
                    Index = 10,
                    Label = "100"
                },
                new Card()
                {
                    Index = 11,
                    Label = "∞"
                },
                new Card()
                {
                    Index = 12,
                    Label = "?"
                },
                new Card()
                {
                    Index = 13,
                    Label = "K"
                }
            };
            return result;
        }
        #endregion
    }
}
