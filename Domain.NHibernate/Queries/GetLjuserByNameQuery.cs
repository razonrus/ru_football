using System.Linq;
using Domain;
using IndyCode.Infrastructure.Domain;

namespace ru_football.Domain.NHibernate.Queries
{
    public class GetLjuserByNameQuery : LinqQueryBase<Ljuser, Ljuser>
    {
        private readonly string name;

        public GetLjuserByNameQuery(ILinqProvider linqProvider, string name) : base(linqProvider)
        {
            this.name = name;
        }

        public override Ljuser Execute()
        {
            return Query().SingleOrDefault(x => x.Name == name);
        }
    }
}