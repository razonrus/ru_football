using System;
using System.Text.RegularExpressions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using IndyCode.Infrastructure.NHibernate;
using NHibernate.Cfg;
using NHibernate.Context;
using ru_football.Domain.NHibernate.Maps;

namespace ru_football.Domain.NHibernate
{
    public class MsSql2008Initializer : INHibernateInitializer
    {
        #region INHibernateInitializer Members

        public Configuration GetConfiguration()
        {
            MsSqlConfiguration config = MsSqlConfiguration.MsSql2008
                .ConnectionString(x => x.FromConnectionStringWithKey("Main"))
                .UseReflectionOptimizer()
                .AdoNetBatchSize(100);

            FluentConfiguration fluentConfiguration = Fluently.Configure()
                .CurrentSessionContext<ThreadStaticSessionContext>()
                .Database(config)
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<LjuserMap>()
                                   .Conventions.AddFromAssemblyOf<TableNameConvention>())
                .ExposeConfiguration(c => c.SetProperty("generate_statistics", "true")
                                              .SetProperty("adonet.batch_size", "100"));

            return fluentConfiguration
                .BuildConfiguration();
        }

        #endregion
    }

    public static class NameConventions
    {
        public static string GetTableName(Type type)
        {
            return ReplaceCamelCaseWithUnderscore(type.Name);
        }

        internal static string ReplaceCamelCaseWithUnderscore(string name)
        {
            return Regex.Replace(name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1_").ToUpper();
        }

        public static string GetSequenceName(Type type)
        {
            return string.Format("{0}_SEQ", GetTableName(type));
        }

        public static string GetPrimaryKeyColumnName(Type type)
        {
            return string.Format("{0}_ID", GetTableName(type));
        }
    }

    public class TableNameConvention : IClassConvention, IJoinedSubclassConvention
    {
        #region IClassConvention Members

        public void Apply(IClassInstance instance)
        {
            string tableName = NameConventions.GetTableName(instance.EntityType);

            instance.Table(tableName);
        }

        #endregion

        #region IJoinedSubclassConvention Members

        public void Apply(IJoinedSubclassInstance instance)
        {
            string tableName = NameConventions.GetTableName(instance.EntityType);

            instance.Table(tableName);
        }

        #endregion
    }
}