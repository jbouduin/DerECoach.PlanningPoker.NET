using DerECoach.PlanningPoker.Core.Domain;
using System.Collections.Generic;

namespace DerECoach.PlanningPoker.Core.Responses
{
    public class JoinResponse
    {
        public Game Game { get; set; }
        public IList<Card> Cards { get; set; }
    }
}
