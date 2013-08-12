using Domain;
using IndyCode.Infrastructure.NHibernate.Mappings;

namespace ru_football.Domain.NHibernate.Maps
{
    public class LjuserMap : EntityMap<Ljuser>
    {
        public LjuserMap()
        {
            Id(x => x.Id).Column("LJUSER_ID");

            Map(x => x.Name);

            HasMany(x => x.Forecasts)
                .Access.ReadOnlyPropertyThroughCamelCaseField()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .AsSet();
        }
    }
}