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

        private readonly IServiceDbRepository dbRepository;
        private readonly ILogger<WeekdayService> logger;

        public WeekdayService(IServiceDbRepository dbRepository, ILogger<WeekdayService> logger)
        {
            this.dbRepository = dbRepository;
            this.logger = logger;
        }


        public async Task<int> CalculateWeekdaysAsync(InputDates input)
        {
            //Validate the input.
            var validErrors = Validate(input);

            if (validErrors <= 0)
            {
                logger.LogInformation($"Input Validation ValidError Code: {validErrors}");
                return validErrors;
            }
            DateTime.TryParse(input.StartDate, System.Globalization.CultureInfo.GetCultureInfo("en-AU"),
                System.Globalization.DateTimeStyles.None, out DateTime Start);
            DateTime.TryParse(input.EndDate, System.Globalization.CultureInfo.GetCultureInfo("en-AU"),
                System.Globalization.DateTimeStyles.None, out DateTime End);

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
            int WeekdaysBeforeAdjustment = CalculateWeekdaysBeforeHolidayAdjustment(End, Start);
            //calculate number of public holidays
            int publicHolidays = await GetCountOfHolidays(Start, End);

            return WeekdaysBeforeAdjustment - publicHolidays;
        }



        public int CalculateWeekdaysBeforeHolidayAdjustment(DateTime End, DateTime Start)
        {
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

            return days - weekEnds;
        }


        private int Validate(InputDates input)
        {
            if (!DateTime.TryParse(input.StartDate, System.Globalization.CultureInfo.GetCultureInfo("en-AU"),
                            System.Globalization.DateTimeStyles.None, out DateTime Start))
            {
                logger.LogInformation($"Bad Start Date: {input.StartDate}");
                return -1;
            }

            if (!DateTime.TryParse(input.EndDate, System.Globalization.CultureInfo.GetCultureInfo("en-AU"),
                System.Globalization.DateTimeStyles.None, out DateTime End))
            {
                logger.LogInformation($"Bad End Date: {input.EndDate}");
                return -2;
            }

            //if start date more than end date.
            if (Start > End)
            {
                logger.LogInformation($"Start Date: {input.StartDate} is greater than End Date: {input.EndDate}");
                return -3;
            }

            //if Start and End Date are same.
            if (Start == End)
            {
                logger.LogInformation($"Start Date: {input.StartDate} is equal to End Date: {input.EndDate}");
                return 0;
            }

            return int.MaxValue;
        }



        private async Task<int> GetCountOfHolidays(DateTime start, DateTime end)
        {
            logger.LogInformation($"Initiating gathering of holiday information for: {start.Year} - {end.Year}");

            //Get the information from Database.
            var holidays = await dbRepository.GetAllHolidaysListAsync();

            List<DateTime> completeHolidayList = new List<DateTime>();

            int count = 0;
            if (start.Year == end.Year)
            {
                //Append
                completeHolidayList.AddRange(await GetHolidayListAsync(holidays, start.Year));
            }
            else
            {
                for (int i = start.Year; i <= end.Year; i++)
                {
                    completeHolidayList.AddRange(await GetHolidayListAsync(holidays, i));
                }
            }

            foreach (var holiday in completeHolidayList)
            {
                if (start < holiday && holiday < end)
                {
                    if (!IsWeekend(holiday))
                        count++;
                }
            }
            return count;
        }




        private async Task<List<DateTime>> GetHolidayListAsync(List<Holidays> holidays, int year)
        {
            List<DateTime> HolidayList = new List<DateTime>();

            foreach (var listItem in holidays)
            {
                //generate the holidays.
                HolidayList.AddRange(await GenerateHolidaysAsync(listItem.HolidayList, year));

            }
            return HolidayList;
        }


        public async Task<List<DateTime>> GenerateHolidaysAsync(IList<Holiday> holidayList, int year)
        {

            List<DateTime> holidayDates = new List<DateTime>();
            foreach (var item in holidayList)
            {
                //check if the date is not null and year is null | Only specific date.
                if (item.Date != null && item.Year == null)
                {
                    var dt = GenerateHolidayBasedOnDate(item, year);
                    dt = CheckforWeekend(dt, item);
                    if (dt != default)
                        holidayDates.Add(dt);

                }

                //check if date is not null and year is not null | Specific date in a specific year.
                if (item.Date != null && item.Year != null)
                {
                    var dt = new DateTime(item.Year ?? default, item.Month ?? default, item.Date ?? default);
                    dt = CheckforWeekend(dt, item);
                    if (dt != default)
                        holidayDates.Add(dt);
                }
                //check if it is week/month based holiday | Monthly Holiday such as Queen's Bday.
                if (item.Date == null && item.WeekCount != null && item.WeekDay != null && item.Month != null)
                {
                    var dt = GenerateHolidayBasedOnWeekMonth(item, year);
                    dt = CheckforWeekend(dt, item);
                    if (dt != default)
                        holidayDates.Add(dt);
                }
            }
            return holidayDates;
        }

        private DateTime CheckforWeekend(DateTime dt, Holiday item)
        {
            var date = dt;
            if (item.Type.Name.ToLower() == "public")
            {
                while (IsWeekend(date))
                {
                    date = date.AddDays(1);
                }

                return date;
            }
            else
            {
                //if not a public holiday, then check for weekends.
                if (!IsWeekend(dt))
                    return dt;
                else
                    return default;
            }
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday
             || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private DateTime GenerateHolidayBasedOnWeekMonth(Holiday item, int year)
        {
            //start 1st day of the month.
            int counterWeekday = item.WeekCount ?? default;
            DateTime dt = new DateTime(year, item.Month ?? default, 1);
            while (dt.Month == item.Month)
            {
                var wk = dt.DayOfWeek.ToString();
                if (wk == item.WeekDay.Name)
                {
                    dt = dt.AddDays(7 * (counterWeekday - 1));
                    return dt;
                }
                else
                    dt = dt.AddDays(1);
            }
            return dt;
        }



        private DateTime GenerateHolidayBasedOnDate(Holiday item, int year)
        {
            return new DateTime(year, item.Month ?? default, item.Date ?? default);
        }
    }
}
