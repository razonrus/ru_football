using System.Collections.Generic;
using Domain;
using IndyCode.Infrastructure.Domain;
using IndyCode.Infrastructure.NHibernate;
using ru_football.Domain.NHibernate.Queries;
using IQueryFactory = Domain.IQueryFactory;

namespace ru_football.Domain.NHibernate
{
    public class QueryFactory : IQueryFactory
    {
        private readonly ILinqProvider linqProvider;
        private readonly ISessionProvider sessionProvider;

        public QueryFactory(ISessionProvider sessionProvider, ILinqProvider linqProvider)
        {
            this.sessionProvider = sessionProvider;
            this.linqProvider = linqProvider;
        }

        #region IQueryFactory Members

        public IQuery<T> FindById<T>(int id) where T : IEntity
        {
            return new FindByIdQuery<T>(sessionProvider, id);
        }

        public IQuery<IEnumerable<T>> FindAll<T>() where T : IEntity
        {
            return new FindAllQuery<T>(linqProvider);
        }

        public IQuery<Ljuser> GetLjuserByName(string name)
        {
            return new GetLjuserByNameQuery(linqProvider, name);
        }

        public IQuery<IList<Forecast>> GetForecastsByNumber(int number)
        {
            return new GetForecastsByNumberQuery(linqProvider, number);
        }

        public IQuery<Match> GetMatchByNumber(int number)
        {
            return new GetMatchByNumberQuery(linqProvider, number);
        }

        public IQuery<Command> GetCommandByName(string name)
        {
            return new GetCommandByNameQuery(linqProvider, name);
        }

        #endregion
    }
}