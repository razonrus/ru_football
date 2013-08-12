using System;
using IndyCode.Infrastructure.Domain;

namespace Domain
{
    public class Forecast : IEntity
    {
        public Forecast()
        {
        }

        public Forecast(Ljuser ljuser)
        {
            Ljuser = ljuser;
        }


        public virtual int Id { get; set; }

        public virtual int Number { get; set; }

        public virtual int OwnersGoals { get; set; }

        public virtual int GuestsGoals { get; set; }

        public virtual Ljuser Ljuser { get; set; }

        public virtual ScoreType Score { get; set; }

        public virtual int GetDifference()
        {
            return OwnersGoals - GuestsGoals;
        }

        public new virtual string ToString()
        {
            return string.Format("{0}:{1}", OwnersGoals, GuestsGoals);
        }

        public virtual string ToTag()
        {
            string tag;
            switch (Score)
            {
                case ScoreType.Wrong:
                    tag = "s";
                    break;
                case ScoreType.Result:
                    return ToString();
                case ScoreType.Difference:
                    tag = "i";
                    break;
                case ScoreType.ScoreMatch:
                    tag = "b";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return string.Format("<{0}>{1}</{0}>", tag, ToString());
        }
    }

    public enum ScoreType
    {
        Wrong = 0,

        /// <summary>
        /// ”гадан результат
        /// </summary>
        Result = 1,

        /// <summary>
        /// ”гадана разница м€чей
        /// </summary>
        Difference = 2,

        /// <summary>
        /// ”гадан счет
        /// </summary>
        ScoreMatch = 4
    }
}