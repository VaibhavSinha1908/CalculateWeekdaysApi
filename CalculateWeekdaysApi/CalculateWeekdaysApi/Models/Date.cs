using Newtonsoft.Json;

namespace CalculateWeekdaysApi.Models
{
    public class Date
    {
        [JsonProperty("StartDate")]
        public string StartDate;

        [JsonProperty("EndDate")]
        public string EndDate;

        public int WeekDaysCount;
    }
}
