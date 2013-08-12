using System.Linq;
using Domain;
using IndyCode.Infrastructure.Domain;

namespace ru_football.Domain.NHibernate
{
    public abstract class LinqQueryBase<TResult, TEntity> : IQuery<TResult>
    {
        private readonly ILinqProvider linqProvider;

        protected LinqQueryBase(ILinqProvider linqProvider)
        {
            this.linqProvider = linqProvider;
        }

        #region IQuery<TResult> Members

        public abstract TResult Execute();

        #endregion

        protected IQueryable<TEntity> Query()
        {
            return linqProvider.Query<TEntity>();
        }
    }
}