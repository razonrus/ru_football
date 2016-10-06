using System.Collections.Generic;
using System.ComponentModel;

namespace ru_football.Models
{
    public class UserStatsModel
    {
        [DisplayName("��� ������������")]
        public string Name { get; set; }

        public Dictionary<int, Dictionary<string, double>> Result { get; set; }
    }
}