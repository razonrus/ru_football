using System.ComponentModel;

namespace ru_football.Models
{
    public class ParseMatchesResultModel
    {
        [DisplayName("Url страницы с результатами")]
        public string Url { get; set; }

        public string Result { get; set; }
    }
}