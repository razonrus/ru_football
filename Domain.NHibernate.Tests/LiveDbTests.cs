using System;
using System.Data.Common;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using ru_football.Domain.NHibernate;

namespace OS.AssetesManagement.Domain.NHibernate.Tests
{
    internal class LiveDbTests
	{
		[Test]
		public void GenerateMigrationScript()
		{
			Configuration configuration = new MsSql2008Initializer().GetConfiguration();
			var factory = (ISessionFactoryImplementor) configuration.BuildSessionFactory();

			using (ISession session = factory.OpenSession())
			{
				string[] updateScripts = configuration.GenerateSchemaUpdateScript(factory.Dialect, new DatabaseMetadata((DbConnection) session.Connection, factory.Dialect));

				foreach (string updateScript in updateScripts)
				{
					Console.WriteLine(updateScript);
					Console.WriteLine("GO");
				}
			}
		}

                [Test]
		public void IsDbInActualState()
		{
			Configuration configuration = new MsSql2008Initializer().GetConfiguration();

			var factory = (ISessionFactoryImplementor) configuration.BuildSessionFactory();

			using (ISession session = factory.OpenSession())
			{
				string[] updateScripts = configuration.GenerateSchemaUpdateScript(factory.Dialect, new DatabaseMetadata((DbConnection) session.Connection, factory.Dialect));
			    Assert.AreEqual(string.Empty, updateScripts);
			}
		}

                [Test]
		public void GenerateCreationScript()
		{
			Configuration configuration = new MsSql2008Initializer().GetConfiguration();
			new SchemaExport(configuration)
				.SetDelimiter("\r\nGO\r\n")
				.Execute(true, false, false);
		}

	}
}