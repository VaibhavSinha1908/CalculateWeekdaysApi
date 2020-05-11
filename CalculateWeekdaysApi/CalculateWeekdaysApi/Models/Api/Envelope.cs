namespace CalculateWeekdaysApi.Models
{
    public class Envelope
    {
        public string api_key { get; set; }
        public string remote_ip { get; set; }
        public string request_url { get; set; }
        public string signature { get; set; }
    }
}