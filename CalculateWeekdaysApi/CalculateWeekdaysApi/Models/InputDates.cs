using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CalculateWeekdaysApi.Models
{
    public class InputDates
    {
        [Required]
        [JsonProperty("StartDate")]
        public string StartDate { get; set; }

        [Required]
        [JsonProperty("EndDate")]
        public string EndDate { get; set; }

    }
}
