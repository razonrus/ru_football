using System.ComponentModel;

namespace ru_football.Models
{
    public class ForecastDto
    {
        [DisplayName("����� �����")]
        public int Number { get; set; }

        [DisplayName("���� ������")]
        public int? OwnersGoals { get; set; }

        [DisplayName("���� ������")]
        public int? GuestsGoals { get; set; }
    }
}