using Domain;
using IndyCode.Infrastructure.NHibernate.Mappings;

namespace ru_football.Domain.NHibernate.Maps
{
    public class CommandMap : EntityMap<Command>
    {
        public CommandMap()
        {
            Id(x => x.Id).Column("Command_ID");

            Map(x => x.Name);

            HasMany(x => x.OwnerMatches)
                .KeyColumn("OWNERS_ID")
                .ForeignKeyConstraintName("FK_MATCH_OWNERSCOMMAND")
                .Access.ReadOnlyPropertyThroughCamelCaseField()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .AsSet();


            HasMany(x => x.GuestMatches)
                .KeyColumn("GUESTS_ID")
                .ForeignKeyConstraintName("FK_MATCH_GUESTSCOMMAND")
                .Access.ReadOnlyPropertyThroughCamelCaseField()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .AsSet();
        }
    }
}