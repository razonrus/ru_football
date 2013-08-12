using System.Collections.Generic;
using System.Linq;
using IndyCode.Infrastructure.Domain;

namespace Domain
{
    public class Ljuser : IEntity
    {
        private readonly IList<Forecast> forecasts;

        public Ljuser()
        {
            forecasts = new List<Forecast>();
        }

        public virtual string Name { get; set; }

        public virtual IEnumerable<Forecast> Forecasts
        {
            get { return forecasts; }
        }

        public virtual int ScoresAfterPreviousTour { get; set; }

        public virtual int Scores { get; set; }

        public virtual int? RankAfterPreviousTour { get; set; }
        public virtual int Rank { get; set; }

        #region IEntity Members

        public virtual int Id { get; set; }

        #endregion

        public virtual int AddForecast(Forecast forecast)
        {
            Forecast existingForecast = forecasts.SingleOrDefault(x => x.Number == forecast.Number);
            if (existingForecast != null)
            {
                existingForecast = forecast;
                return 0;
            }
            forecasts.Add(forecast);
            return 1;
        }

        public virtual int AddForecasts(IEnumerable<Forecast> parsedForecasts)
        {
            return parsedForecasts.Sum(forecast => AddForecast(forecast));
        }
    }
}