
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace CalculateWeekdaysApi.Models
{
    public class Holiday
    {


        [Key]
        public int Id { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("end")]
        public DateTime End { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("public")]
        public bool IsPublic { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("substitute")]
        public bool Substitute { get; set; }
    }
}