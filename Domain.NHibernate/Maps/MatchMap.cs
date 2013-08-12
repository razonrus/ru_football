using Domain;
using IndyCode.Infrastructure.NHibernate.Mappings;

namespace ru_football.Domain.NHibernate.Maps
{
    public class MatchMap : EntityMap<Match>
    {
        public MatchMap()
        {
            Id(x => x.Id).Column("Match_ID");

            Map(x => x.Number);
            Map(x => x.OwnersGoals).Column("OWNERs_GOALS");
            Map(x => x.GuestsGoals).Column("Guests_GOALS");
            Map(x => x.Date);
            References(x => x.Owners);
            References(x => x.Guests);
        }
    }
}