using Domain;
using IndyCode.Infrastructure.NHibernate;
using NHibernate;

namespace ru_football.Domain.NHibernate
{
    public abstract class SessionQueryBase<TResult> : IQuery<TResult>
    {
        private readonly ISessionProvider sessionProvider;

        protected SessionQueryBase(ISessionProvider sessionProvider)
        {
            this.sessionProvider = sessionProvider;
        }

        #region IQuery<TResult> Members

        public abstract TResult Execute();

        #endregion

        protected ISession Session()
        {
            return sessionProvider.CurrentSession;
        }
    }
}