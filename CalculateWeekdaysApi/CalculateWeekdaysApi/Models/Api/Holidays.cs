using System.Collections.Generic;

namespace CalculateWeekdaysApi.Models.ApiResponse
{
    public class Holidays
    {
        public Query query { get; set; }
        public List<Holiday> holidays { get; set; }
    }
}