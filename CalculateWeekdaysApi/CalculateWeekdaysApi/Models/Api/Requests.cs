using System;

namespace CalculateWeekdaysApi.Models.ApiResponse
{
    public class Requests
    {
        public int used { get; set; }
        public int available { get; set; }
        public DateTime resets { get; set; }
    }
}