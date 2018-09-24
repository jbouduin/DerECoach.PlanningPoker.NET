using DerECoach.Common.BaseTypes;
using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using DerECoach.PlanningPoker.Core.Responses;
using DerECoach.PlanningPoker.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.WebSockets
{
    public class GameHub: Hub
    {
        #region game related --------------------------------------------------
        public async Task<Result<HttpStatusCode, string, JoinResponse>> Join(JoinRequest request)
        {            
            
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
            var result =  new JoinResponse();
            result.Game = await GameService.GetInstance().JoinGameAsync(request, Context.ConnectionId);
            result.Cards = GetCards();
            var participant = await result.Game.GetParticipantByScreenNameAsync(request.ScreenName);
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("joined", participant);
            
            return Result<HttpStatusCode, string, JoinResponse>.Success(result);
        }

        public async Task<Result<HttpStatusCode, string, JoinResponse>> Rejoin(JoinRequest request)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
            var result = new JoinResponse();
            result.Game = await GameService.GetInstance().RejoinGameAsync(request, Context.ConnectionId);
            result.Cards = GetCards();
            var participant = await result.Game.GetParticipantByScreenNameAsync(request.ScreenName);
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("rejoined", participant);
            return Result<HttpStatusCode, string, JoinResponse>.Success(result);
        }

        public async Task<Result<HttpStatusCode, string, CreateResponse>> Create(CreateRequest request)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);

            var result = await GameService.GetInstance().CreateGameAsync(request, Context.ConnectionId);

            return result.Return(gameResult =>
            {
                var response = new CreateResponse();
                response.Cards = GetCards();
                response.Game = gameResult.Value;
                return response;
            });
        }

        public async Task<Result<HttpStatusCode, string>> Leave(LeaveRequest leaveRequest)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, leaveRequest.TeamName);
            var participant = GameService.GetInstance().LeaveGame(leaveRequest);
            await Clients.Group(leaveRequest.TeamName).SendAsync("left", participant);
            return Result<HttpStatusCode, string>.Success();
        }

        public async Task<Result<HttpStatusCode, string>> Start(string teamName)
        {
            await GameService.GetInstance().StartGameAsync(teamName);
            await Clients.GroupExcept(teamName, Context.ConnectionId).SendAsync("started");
            return Result<HttpStatusCode, string>.Success();
        }

        public async Task<Result<HttpStatusCode, string>> End(EndRequest request)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, request.TeamName);
            GameService.GetInstance().EndGame(request);
            await Clients.Group(request.TeamName).SendAsync("ended");
            return Result<HttpStatusCode, string>.Success();
        }
        #endregion

        #region estimations related -------------------------------------------
        public async Task<Result<HttpStatusCode, string>> Estimate(EstimateRequest request)
        {            
            var estimation = await GameService.GetInstance().EstimateAsync(request);            
            await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("estimated", estimation);
            return Result<HttpStatusCode, string>.Success();
        }
        #endregion

        #region overrides -----------------------------------------------------
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception).ContinueWith((task) => ProcessOnDisconnected(Context.ConnectionId));
            
        }

        #endregion

        #region helpers -------------------------------------------------------       
        private void ProcessOnDisconnected(string connectionId)
        {            
            var team = GameService.GetInstance().GetTeamByConnectionId(connectionId);
            var participant = GameService.GetInstance().OnDisconnected(connectionId);
            Clients.GroupExcept(team, Context.ConnectionId).SendAsync("disconnected", participant);
        }

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
