using System.Linq;
using Domain;
using IndyCode.Infrastructure.Domain;

namespace ru_football.Domain.NHibernate.Queries
{
    public class GetMatchByNumberQuery : LinqQueryBase<Match, Match>
    {
        private readonly int number;

        public GetMatchByNumberQuery(ILinqProvider linqProvider, int number) : base(linqProvider)
        {
            this.number = number;
        }

        public override Match Execute()
        {
            return Query().Where(x => x.Number == number).SingleOrDefault();
        }
    }
}