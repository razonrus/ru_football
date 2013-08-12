using IndyCode.Infrastructure.Domain;
using IndyCode.Infrastructure.NHibernate;

namespace ru_football.Domain.NHibernate
{
    public class FindByIdQuery<T> : SessionQueryBase<T> where T : IEntity
    {
        private readonly int id;

        public FindByIdQuery(ISessionProvider sessionProvider, int id) : base(sessionProvider)
        {
            this.id = id;
        }

        public override T Execute()
        {
            return Session().Get<T>(id);
        }
    }
}