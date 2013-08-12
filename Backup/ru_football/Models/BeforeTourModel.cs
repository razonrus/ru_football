using System.ComponentModel;

namespace ru_football.Models
{
    public class BeforeTourModel
    {
        [DisplayName("Номера матчей тура через запятую")]
        public string Numbers { get; set; }

        public string Result { get; set; }
    }
}