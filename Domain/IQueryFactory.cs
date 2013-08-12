using System.Collections.Generic;
using IndyCode.Infrastructure.Domain;

namespace Domain
{
    public interface IQueryFactory
    {
        /// <summary>
        /// Создает запрос для выборки сущности типа <see cref="TEntity"/> по <see cref="IEntity.Id"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        IQuery<TEntity> FindById<TEntity>(int id) where TEntity : IEntity;

        /// <summary>
        /// Создает запрос для выборки из базы всех сущностей типа <see cref="TEntity"/>
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        IQuery<IEnumerable<TEntity>> FindAll<TEntity>() where TEntity : IEntity;

        IQuery<Ljuser> GetLjuserByName(string name);

        IQuery<IList<Forecast>> GetForecastsByNumber(int number);
        IQuery<Match> GetMatchByNumber(int number);

        IQuery<Command> GetCommandByName(string name);
    }
}