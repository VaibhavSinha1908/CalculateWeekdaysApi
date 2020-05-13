using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Models.Entities;
using CalculateWeekdaysApi.Repositories;
using CalculateWeekdaysApi.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Tests
{
    public class WeekDayServiceTests
    {
        //Members

        private WeekdayService _weekDayService { get; set; }

        public ILogger<WeekdayService> _loggerWeekdayService { get; private set; }

        [SetUp]
        public void Setup()
        {
            var mockLoggerWeekday = new Mock<ILogger<WeekdayService>>();



            var mockDbRepository = new Mock<IServiceDbRepository>();


            _weekDayService = new WeekdayService(mockDbRepository.Object, mockLoggerWeekday.Object);

        }



        [TestCase("13/8/2015", "31/12/2015", 97)]
        [TestCase("13/8/2015", "21/8/2015", 5)]
        [TestCase("07/8/2015", "11/8/2015", 1)]
        public async Task CalculateWeekDays_ForValidInputs_ReturnsCountOfWeekDays(string startDate, string endDate, int expectedOutput)
        {
            //Arrange
            int result = 0;
            DateTime.TryParse(startDate, out DateTime start);
            DateTime.TryParse(endDate, out DateTime end);
            var input = new InputDates
            {
                StartDate = startDate,
                EndDate = endDate
            };


            //Create In Memory Context
            List<Holiday> holidays = GetHolidayList();

            var options = new DbContextOptionsBuilder<HolidayContext>()
                .UseLazyLoadingProxies()
                .UseInMemoryDatabase(databaseName: "CustomDbInMemory")
                .Options;

            // Seed data into the database using one instance of the context
            using (var context = new HolidayContext(options))
            {
                if (context.Holidays.Count() == 0)
                {
                    context.Holidays.Add(new Holidays { Name = "My_TEST_HolidayList", HolidayList = holidays });
                    context.SaveChanges();
                }

                var holidayList = context.Holidays.ToList().FirstOrDefault().HolidayList;

                var generatedHolidayList = await _weekDayService.GenerateHolidaysAsync(holidayList, start.Year);
                generatedHolidayList.AddRange(await _weekDayService.GenerateHolidaysAsync(holidayList, end.Year));

                var validHolidays = generatedHolidayList.Where(x => x > start && x < end).Count();


                // ACT
                result = _weekDayService.CalculateWeekdaysBeforeHolidayAdjustment(end, start) - validHolidays;
            }

            //ASSERT
            Assert.That(result == expectedOutput);
            return;
        }



        [TestCase("", "21/8/2016", -1)]
        [TestCase(null, "21/8/2016", -1)]
        [TestCase(null, null, -1)]
        [TestCase("", "", -1)]
        [TestCase("13/8/2015", "", -2)]
        [TestCase("13/8/2015", "13/8/2015", 0)]
        [TestCase("13/8/2015", "13/8/2014", -3)]
        public async Task CalculateWeekDays_ForInValidInputs_ReturnsErrorCodes(string startDate, string endDate, int expectedOutput)
        {
            var input = new InputDates
            {
                StartDate = startDate,
                EndDate = endDate
            };
            var result = await _weekDayService.CalculateWeekdaysAsync(input);

            Assert.That(result == expectedOutput);
        }



        private static List<Holiday> GetHolidayList()
        {

            //setup holiday list for 2015.
            return new List<Holiday>
            {
                new Holiday
                {

                    Name = "Queen's Bday",

                    Month = 6,
                    WeekCount = 2,
                    WeekDay = new WeekDays
                    {
                        Index = 1,
                        Name = "Monday"
                    },
                    Date = null,
                    Type = new HolidayType
                    {
                        Name = "public"
                    },
                    Year = null
                },

                new Holiday
                {

                    Name = "Christmas",

                    Month = 12,
                    WeekCount = null,
                    WeekDay = null,
                    Date = 25,
                    Type = new HolidayType
                    {
                        Name = "public"
                    },
                    Year = null
                },
            };
        }


    }
}
