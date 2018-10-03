using DerECoach.Common.BaseTypes;
using DerECoach.PlanningPoker.Core.Domain;
using DerECoach.PlanningPoker.Core.Requests;
using DerECoach.PlanningPoker.Core.Responses;
using DerECoach.PlanningPoker.Core.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DerECoach.PlanningPoker.Core.WebSockets
{
    public class GameHub : Hub
    {
        #region private fields ------------------------------------------------
        private readonly IResultFactory _resultFactory = ResultFactoryProvider.GetResultFactory();
        private readonly IValueResultFactory _valueResultFactory = ValueResultFactoryProvider.GetValueResultFactory();
        #endregion

        #region game related --------------------------------------------------
        public async Task<IValueResult<JoinResponse>> Join(JoinRequest request)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
                var result = await GameService.GetInstance().JoinGameAsync(request, Context.ConnectionId);

                var join = result
                    .Convert(joinResult =>
                    {
                        var response = new JoinResponse
                        {
                            Cards = GetCards(),
                            Game = joinResult.Value
                        };
                        return response;
                    });

                if (join.Succeeded)
                {
                    var participant = await join.Value.Game.GetParticipantByScreenNameAsync(request.ScreenName);
                    await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("joined", participant);
                }
                return join;
            }
            catch
            {
                return _valueResultFactory.Failure<JoinResponse>(null, "Some error occurred on the server");
            }
        }

        public async Task<IValueResult<JoinResponse>> Rejoin(JoinRequest request)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);
                var result = await GameService.GetInstance().RejoinGameAsync(request, Context.ConnectionId);

                var join = result.Convert(
                    joinResult =>
                    {
                        var response = new JoinResponse
                        {
                            Cards = GetCards(),
                            Game = joinResult.Value
                        };
                        return response;
                    });

                if (join.Succeeded)
                {
                    var participant = await join.Value.Game.GetParticipantByScreenNameAsync(request.ScreenName);
                    await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("rejoined", participant);
                }
                return join;
            }
            catch
            {
                return _valueResultFactory.Failure<JoinResponse>(null, "Some error occurred on the server");
            }
        }

        public async Task<IValueResult<CreateResponse>> Create(CreateRequest request)
        {
            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, request.TeamName);

                var result = await GameService.GetInstance().CreateGameAsync(request, Context.ConnectionId);

                return result.Convert(gameResult =>
                {
                    var response = new CreateResponse
                    {
                        Cards = GetCards(),
                        Game = gameResult.Value
                    };
                    return response;
                });
            }
            catch
            {
                return _valueResultFactory.Failure<CreateResponse>(null, "Some error occurred on the server");
            }
        }

        public async Task<IResult> Leave(LeaveRequest leaveRequest)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, leaveRequest.TeamName);
                var participant = GameService.GetInstance().LeaveGame(leaveRequest, Context.ConnectionId);
                await Clients.Group(leaveRequest.TeamName).SendAsync("left", participant);
                return _resultFactory.Success();
            }
            catch
            {
                return _resultFactory.Failure("Some error occurred on the server");
            }
        }

        public async Task<IResult> Start(string teamName)
        {
            try
            {
                await GameService.GetInstance().StartGameAsync(teamName);
                await Clients.GroupExcept(teamName, Context.ConnectionId).SendAsync("started");
                return _resultFactory.Success();
            }
            catch
            {
                return _resultFactory.Failure("Some error occurred on the server");
            }
        }

        public async Task<IResult> End(EndRequest request)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, request.TeamName);
                GameService.GetInstance().EndGame(request);
                await Clients.Group(request.TeamName).SendAsync("ended");
                return _resultFactory.Success();
            }
            catch
            {
                return _resultFactory.Failure("Some error occurred on the server");
            }
        }
        #endregion

        #region estimations related -------------------------------------------
        public async Task<IResult> Estimate(EstimateRequest request)
        {
            try
            {
                var estimation = await GameService.GetInstance().EstimateAsync(request);
                await Clients.GroupExcept(request.TeamName, Context.ConnectionId).SendAsync("estimated", estimation);
                return _resultFactory.Success();
            }
            catch
            {
                return _resultFactory.Failure("Some error occurred on the server");
            }
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
