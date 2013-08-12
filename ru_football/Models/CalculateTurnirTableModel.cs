using System.ComponentModel;

namespace ru_football.Models
{
    public class CalculateTurnirTableModel
    {
        [DisplayName("Номер последнего матча текущего тура")]
        public int LastMatchNumber { get; set; }

        [DisplayName("Номер последнего матча предыдущего тура")]
        public int LastMatchNumberOfPreviousTour { get; set; }

        public string Result { get; set; }
    }
}