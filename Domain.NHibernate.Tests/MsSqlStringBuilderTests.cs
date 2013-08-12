using Xunit;
using ru_football.Domain.NHibernate;

namespace OS.AssetesManagement.Domain.NHibernate.Tests
{
	public class MsSqlStringBuilderTests
	{
		[Fact]
		public void TestContainsPatternBuilding()
		{
			var result = MsSqlStringBuilder.AddContainsPattern("word");

			Assert.Equal("\"word*\"", result);
		}

		[Fact]
		public void TestContainsPatternForMoreThanOneWordBuilding()
		{
			var result = MsSqlStringBuilder.AddContainsPattern("world is mine");

			Assert.Equal("\"world*\"AND\"is*\"AND\"mine*\"", result);
		}

		[Fact]
		public void RemoveWhitespacesFromValue()
		{
			var result = MsSqlStringBuilder.AddContainsPattern("world		 is	 	\n\r	mine");

			Assert.Equal("\"world*\"AND\"is*\"AND\"mine*\"", result);
		}
	}
}