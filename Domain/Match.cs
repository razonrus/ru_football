using System;
using IndyCode.Infrastructure.Domain;

namespace Domain
{
    public class Match : IEntity
    {
        public virtual int Number { get; set; }

        public virtual int? OwnersGoals { get; set; }

        public virtual int? GuestsGoals { get; set; }

        public virtual Command Owners { get; set; }

        public virtual Command Guests { get; set; }

        public virtual DateTime? Date { get; set; }

        #region IEntity Members

        public virtual int Id { get; set; }

        #endregion

        public virtual int GetDifference()
        {
            return OwnersGoals.Value - GuestsGoals.Value;
        }

        public new virtual string ToString()
        {
            return string.Format("{0}:{1}", OwnersGoals, GuestsGoals);
        }

        public virtual string Caption()
        {
            return string.Format("{0} - {1}", Owners.Name, Guests.Name);
        }

        public virtual bool IsOver()
        {
            return GuestsGoals.HasValue && OwnersGoals.HasValue;
        }
    }
}