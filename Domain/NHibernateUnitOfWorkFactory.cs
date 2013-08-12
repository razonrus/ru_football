using System;
using System.Data;
using IndyCode.Infrastructure.Domain;
using NHibernate;
using NHibernate.Context;

public class NHibernateUnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly ISessionFactory sessionSessionFactory;

    ///<summary>
    /// ctor
    ///</summary>
    ///<param name="sessionFactory"></param>
    public NHibernateUnitOfWorkFactory(ISessionFactory sessionFactory)
    {
        sessionSessionFactory = sessionFactory;
    }

    #region IUnitOfWorkFactory Members

    public IUnitOfWork Create(IsolationLevel isolationLevel)
    {
        return new NHibernateUnitOfWork(sessionSessionFactory.OpenSession(), TransactionBehavior.Rollback,
                                        isolationLevel);
    }

    public IUnitOfWork Create()
    {
        return Create(IsolationLevel.ReadCommitted);
    }

    #endregion

    public IUnitOfWork Create(TransactionBehavior transactionBehavior)
    {
        return new NHibernateUnitOfWork(sessionSessionFactory.OpenSession(), transactionBehavior);
    }
}

internal class NHibernateUnitOfWork : IUnitOfWork
{
    private readonly ISession session;
    private readonly TransactionBehavior transactionBehavior;
    private ITransaction transaction;

    public NHibernateUnitOfWork(ISession session, TransactionBehavior transactionBehavior,
                                IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        if (session == null)
            throw new ArgumentNullException("session");

        CurrentSessionContext.Bind(session);

        this.session = session;
        transaction = session.BeginTransaction(isolationLevel);

        this.transactionBehavior = transactionBehavior;
    }

    #region IUnitOfWork Members

    public void Dispose()
    {
        if (!transaction.WasCommitted && !transaction.WasRolledBack)
        {
            if (transactionBehavior == TransactionBehavior.Commit)
                transaction.Commit();

            if (transactionBehavior == TransactionBehavior.Rollback)
                transaction.Rollback();
        }
        transaction.Dispose();
        transaction = null;

        CurrentSessionContext.Unbind(session.SessionFactory);
        session.Dispose();
    }

    public void Commit()
    {
        transaction.Commit();
    }

    public void Save<TEntity>(TEntity entity) where TEntity : IEntity
    {
        session.Save(entity);
    }

    public void Delete<TEntity>(TEntity entity) where TEntity : IEntity
    {
        session.Delete(entity);
    }

    #endregion
}

public enum TransactionBehavior
{
    Commit,
    Rollback
}