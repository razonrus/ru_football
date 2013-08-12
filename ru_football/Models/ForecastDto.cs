using System.ComponentModel;

namespace ru_football.Models
{
    public class ForecastDto
    {
        [DisplayName("Номер матча")]
        public int Number { get; set; }

        [DisplayName("Голы хозяев")]
        public int? OwnersGoals { get; set; }

        [DisplayName("Голы гостей")]
        public int? GuestsGoals { get; set; }
    }
}