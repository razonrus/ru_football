using System.Collections.Generic;
using System.Linq;
using Domain;
using IndyCode.Infrastructure.Domain;
using NHibernate.Linq;

namespace ru_football.Domain.NHibernate.Queries
{
    public class GetForecastsByNumberQuery : LinqQueryBase<IList<Forecast>, Forecast>
    {
        private readonly int number;

        public GetForecastsByNumberQuery(ILinqProvider linqProvider, int number) : base(linqProvider)
        {
            this.number = number;
        }

        public override IList<Forecast> Execute()
        {
            return Query().Where(x => x.Number == number).Fetch(x => x.Ljuser).ToList();
        }
    }
}