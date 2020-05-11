namespace CalculateWeekdaysApi.Models.ApiResponse
{
    public sealed class ApiConfiguration
    {
        public readonly string ApiKey;
        public readonly string BaseUrl;
        public readonly string QueryParams;

        public ApiConfiguration(string apiKey, string baseUrl, string queryParams)
        {
            this.ApiKey = apiKey;
            this.BaseUrl = baseUrl;
            this.QueryParams = queryParams;
        }
    }
}
