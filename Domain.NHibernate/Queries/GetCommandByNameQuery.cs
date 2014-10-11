using System.Linq;
using Domain;
using IndyCode.Infrastructure.Domain;

namespace ru_football.Domain.NHibernate.Queries
{
    public class GetCommandByNameQuery : LinqQueryBase<Command, Command>
    {
        private readonly string name;

        public GetCommandByNameQuery(ILinqProvider linqProvider, string name) : base(linqProvider)
        {
            if (name == "Арсенал")
                name = "Арсенал Тула";

            this.name = name;
        }

        public override Command Execute()
        {
            return Query().Single(x => x.Name == name);
        }
    }
}