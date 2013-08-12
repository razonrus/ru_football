using System;
using System.Data;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using IndyCode.Infrastructure.Domain;
using IndyCode.Infrastructure.NHibernate;
using NHibernate;
using NHibernate.Context;
using NHibernate.Engine;
using log4net;
using ru_football.Domain.NHibernate;
using IQueryFactory = Domain.IQueryFactory;

namespace ru_football
{
    public class WindsorInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
//            ILog log = LogManager.GetLogger("logger");
//            XmlConfigurator.Configure();

            container.Register(
                Component.For<IQueryFactory>().ImplementedBy<QueryFactory>(),
                Component.For<ILog>().ImplementedBy<LogWrapper>(),
                Component.For<ICalculator>().ImplementedBy<Calculator>()
                );

//            container.Register(Component.For<ISessionProvider>().ImplementedBy<SessionProvider>(),
//                               Component.For<ILinqProvider>().ImplementedBy<NHibernateLinqProvider>(),
//                               Component.For<IUnitOfWorkFactory>().ImplementedBy<NHibernateUnitOfWorkFactory>(),
//                               Component.For<INHibernateInitializer>().ImplementedBy<MsSql2008Initializer>(),
//                               Component.For<ISessionFactory>().UsingFactoryMethod(
//                                   x => x.Resolve<INHibernateInitializer>().GetConfiguration().BuildSessionFactory())
//                                   .LifeStyle.Is(LifestyleType.Singleton))
            ;
        }

        #endregion
    }

    public class NHibernateInstaller : IWindsorInstaller
    {
        #region IWindsorInstaller Members

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ISessionProvider>().ImplementedBy<SessionProvider>(),
                               Component.For<ILinqProvider>().ImplementedBy<NHibernateLinqProvider>(),
                               Component.For<IUnitOfWorkFactory>().ImplementedBy<NHibernateUnitOfWorkFactory>(),
                               Component.For<INHibernateInitializer>().ImplementedBy<MsSql2008Initializer>(),
                               Component.For<ISessionFactory>().UsingFactoryMethod(BuildSessionFactory)
                                   .LifeStyle.Is(LifestyleType.Singleton));
        }

        #endregion

        private static ISessionFactory BuildSessionFactory(IKernel x)
        {
            var sessionFactory =
                (ISessionFactoryImplementor)
                x.Resolve<INHibernateInitializer>().GetConfiguration().BuildSessionFactory();
            return sessionFactory;
        }
    }
}