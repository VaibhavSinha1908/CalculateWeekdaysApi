using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Models.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Repositories.Implementations
{
    public class ServiceDbRepository : IServiceDbRepository
    {
        private readonly HolidayContext context;
        private readonly ILogger<ServiceDbRepository> logger;

        public ServiceDbRepository(HolidayContext context, ILogger<ServiceDbRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }



        public async Task<List<Holidays>> GetAllHolidaysListAsync()
        {
            List<Holidays> result;

            try
            {
                logger.LogInformation($"Accessing DbContext to get the Holiday information");

                result = context.Holidays.ToList();
                return result;
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
