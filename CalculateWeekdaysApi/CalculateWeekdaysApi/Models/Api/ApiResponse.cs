namespace CalculateWeekdaysApi.Models.ApiResponse
{
    public class ApiResponse
    {
        public int status { get; set; }
        public Envelope envelope { get; set; }
        public Requests requests { get; set; }
        public string message { get; set; }
        public object errors { get; set; }
        public Holidays holidays { get; set; }
        public Availablefilters availableFilters { get; set; }
    }
}