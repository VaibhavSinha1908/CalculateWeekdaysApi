using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Services.Implementation
{
    public class WeekdayService : IWeekdayService
    {
        private readonly IServiceApiRepository apirepository;
        private readonly IServiceDbRepository dbRepository;
        private readonly ILogger<WeekdayService> logger;

        public WeekdayService(IServiceApiRepository apiRepository, IServiceDbRepository dbRepository, ILogger<WeekdayService> logger)
        {
            this.apirepository = apiRepository;
            this.dbRepository = dbRepository;
            this.logger = logger;
        }



        public async Task<int> CalculateWeekdaysAsync(InputDates input)
        {
            //Validate the input.
            var validErrors = Validate(input);

            if (validErrors <= 0)
                return validErrors;

            DateTime.TryParse(input.StartDate, out DateTime Start);
            DateTime.TryParse(input.EndDate, out DateTime End);

            //do not include start and end date.
            int days = (int)(End - Start).TotalDays - 1;

            int weekEnds = days / 7 * 2; //Number of Weekends in a 7 day week.
            int remain = days % 7;
            DateTime dt = End.AddDays(-remain);
            while (dt.Date < End.Date)
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                    weekEnds++;
                dt = dt.AddDays(1);
            }

            //calculate number of public holidays
            int publicHolidays = await GetCountOfPublicHolidaysAsync(Start, End);

            return days - weekEnds - publicHolidays;
        }


        private int Validate(InputDates input)
        {
            if (!DateTime.TryParse(input.StartDate, out DateTime Start))
                return -1;

            if (!DateTime.TryParse(input.EndDate, out DateTime End))
                return -2;

            //if start date more than end date.
            if (Start > End)
                return -3;

            //if Start and End Date are same.
            if (Start == End)
                return 0;

            return int.MaxValue;
        }



        private async Task<int> GetCountOfPublicHolidaysAsync(DateTime start, DateTime end)
        {
            logger.LogInformation($"Initiating gathering of public information for: {start.Year} - {end.Year}");

            List<Holiday> publicHolidayList = new List<Holiday>();
            int count = 0;
            if (start.Year == end.Year)
            {
                //Append
                publicHolidayList.AddRange(await GetHolidayListAsync(start.Year));
            }
            else
            {
                for (int i = start.Year; i <= end.Year; i++)
                {
                    var holidayList = await GetHolidayListAsync(i);
                    publicHolidayList.AddRange(holidayList);
                }
            }

            foreach (var holiday in publicHolidayList)
            {
                //start the count only when public holiday.
                if (holiday.IsPublic)
                {
                    if (DateTime.TryParse(holiday.Date, out DateTime date))
                    {
                        if (date > start && date < end)
                        {
                            //if the holiday is substitute (i.e. on a Weekday(or Monday)).
                            if (holiday.Substitute == true)
                            {
                                count++;
                            }
                            else
                            {
                                //if the public holiday is not on weekends.
                                if (date.DayOfWeek != DayOfWeek.Saturday &&
                                    date.DayOfWeek != DayOfWeek.Sunday)
                                {
                                    count++;
                                }
                            }
                        }
                    }
                }
            }
            return count;
        }


        private async Task<IEnumerable<Holiday>> GetHolidayListAsync(int year)
        {
            try
            {
                IEnumerable<Holiday> holidays;

                //check if the info is present in database.
                holidays = await dbRepository.GetHolidaysListAsync(year);

                //Check if Holidays is null; Call Api to get the public holidays info.
                if (holidays == null)
                {
                    //get the public holiday list from Api.
                    holidays = await apirepository.GetHolidaysList(year);

                    if (holidays != null)
                    {
                        //upsert the info in database.
                        if (await UpdateDataStoreAsync(holidays, year))
                        {
                            logger.Log(LogLevel.Information, $"Upserted {year} info in the database");
                        }
                    }

                }
                return holidays;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.StackTrace);
                throw;
            }
        }


        //Update the database with new Year and Holiday List.
        private async Task<bool> UpdateDataStoreAsync(IEnumerable<Holiday> holidays, int year)
        {
            return await dbRepository.UpdateHolidaysAsync(holidays, year);
        }
    }
}
