using System.ComponentModel;

namespace ru_football.Models
{
    public class ParseUrlModel
    {
        [DisplayName("Url страницы с прогнозами")]
        public string Url { get; set; }

        public string Result { get; set; }
    }
}