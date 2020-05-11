using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Models.ApiResponse;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Repositories.Implementations
{
    public class ServiceApiRepository : IServiceApiRepository
    {
        private readonly HttpClient httpClient;
        private readonly ApiConfiguration apiConfiguration;
        private readonly ILogger<ServiceApiRepository> logger;
        private readonly string _remoteServiceBaseUrl;

        public ServiceApiRepository(HttpClient httpClient, ApiConfiguration apiConfiguration, ILogger<ServiceApiRepository> logger)
        {
            this.httpClient = httpClient;
            this.apiConfiguration = apiConfiguration;
            this.logger = logger;
            this._remoteServiceBaseUrl = string.Empty;
        }

        public async Task<IEnumerable<Holiday>> GetHolidaysList(int year)
        {
            try
            {
                logger.LogInformation($"Calling FestivoApi for: {year}");
                ApiResponse response;
                var uri = $"{apiConfiguration.BaseUrl}{apiConfiguration.ApiKey}{apiConfiguration.QueryParams}{year}";

                var responseString = await httpClient.GetStringAsync(uri);

                if (responseString != null)
                {
                    response = JsonConvert.DeserializeObject<ApiResponse>(responseString);
                    return response.holidays.holidays;
                }
            }
            catch (WebException ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }

            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }

            return null;

        }


    }
}
