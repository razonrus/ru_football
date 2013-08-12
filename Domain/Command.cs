using System.Collections.Generic;
using IndyCode.Infrastructure.Domain;

namespace Domain
{
    public class Command : IEntity
    {
        private readonly IList<Match> guestMatches;
        private readonly IList<Match> ownerMatches;

        public Command()
        {
            ownerMatches = new List<Match>();
            guestMatches = new List<Match>();
        }

        public virtual IEnumerable<Match> OwnerMatches
        {
            get { return ownerMatches; }
        }

        public virtual IEnumerable<Match> GuestMatches
        {
            get { return guestMatches; }
        }

        public virtual string Name { get; set; }

        #region IEntity Members

        public virtual int Id { get; set; }

        #endregion
    }
}