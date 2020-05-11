using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
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



        public async Task<IEnumerable<Holiday>> GetHolidaysListAsync(int year)
        {
            try
            {
                logger.LogInformation($"Calling DbContext to get the information for: {year}");

                var result = await context.PublicHolidays.Where(x => x.Year == year).Include(h => h.Holidays).FirstOrDefaultAsync();

                if (result == null)
                    return null;

                return result.Holidays;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public async Task<bool> UpdateHolidaysAsync(IEnumerable<Holiday> holidays, int year)
        {
            try
            {
                logger.LogInformation($"Calling DbContext to upsert information for: {year}");
                if (holidays != null)
                {
                    var entity = new PublicHolidays
                    {
                        Year = year,
                        Holidays = holidays.ToList()
                    };
                    await context.PublicHolidays.AddAsync(entity);
                    await context.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }
    }
}
