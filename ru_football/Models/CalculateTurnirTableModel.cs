using System.ComponentModel;

namespace ru_football.Models
{
    public class CalculateTurnirTableModel
    {
        [DisplayName("����� ���������� ����� �������� ����")]
        public int LastMatchNumber { get; set; }

        [DisplayName("����� ���������� ����� ����������� ����")]
        public int LastMatchNumberOfPreviousTour { get; set; }

        public string Result { get; set; }
    }
}