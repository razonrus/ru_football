using System.Collections.Generic;
using System.Linq;
using IndyCode.Infrastructure.Domain;

namespace ru_football.Domain.NHibernate
{
    public class FindAllQuery<T> : LinqQueryBase<IEnumerable<T>, T>
    {
        public FindAllQuery(ILinqProvider linqProvider) : base(linqProvider)
        {
        }

        public override IEnumerable<T> Execute()
        {
            return Query().ToList();
        }
    }
}