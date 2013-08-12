using Domain;
using IndyCode.Infrastructure.NHibernate.Mappings;

namespace ru_football.Domain.NHibernate.Maps
{
    public class ForecastMap : EntityMap<Forecast>
    {
        public ForecastMap()
        {
            Id(x => x.Id).Column("Forecast_ID");

            Map(x => x.Number);
            Map(x => x.OwnersGoals).Column("OWNERs_GOALS");
            Map(x => x.GuestsGoals).Column("Guests_GOALS");
            References(x => x.Ljuser);
        }
    }
}