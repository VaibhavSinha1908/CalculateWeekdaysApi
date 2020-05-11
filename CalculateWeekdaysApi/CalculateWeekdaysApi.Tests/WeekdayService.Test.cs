using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Models.Entities;
using CalculateWeekdaysApi.Repositories;
using CalculateWeekdaysApi.Repositories.Implementations;
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
    public class Tests
    {
        //Members

        private WeekdayService _weekDayService { get; set; }
        private HolidayContext context;

        public ILogger<WeekdayService> _loggerWeekdayService { get; private set; }

        [SetUp]
        public void Setup()
        {
            //create an inMemoryContext

            var mockLoggerWeekday = new Mock<ILogger<WeekdayService>>();

            var mockLoggerApiRespository = new Mock<ILogger<ServiceApiRepository>>();

            var mockApiRepository = new Mock<IServiceApiRepository>();
            mockApiRepository.Setup(x => x.GetHolidaysList(It.IsAny<int>()))
                .Returns(Task.FromResult(It.IsAny<IEnumerable<Holiday>>()));


            var mockDbRepository = new Mock<IServiceDbRepository>();
            mockApiRepository.Setup(x => x.GetHolidaysList(It.IsAny<int>()))
                .Returns(Task.FromResult(It.IsAny<IEnumerable<Holiday>>()));


            _weekDayService = new WeekdayService(mockApiRepository.Object, mockDbRepository.Object, mockLoggerWeekday.Object);

        }



        [TestCase("13/8/2015", "31/12/2015", 2015, 97)]
        [TestCase("13/8/2015", "21/8/2015", 2015, 5)]
        [TestCase("07/8/2015", "11/8/2015", 2015, 1)]
        public async Task CalculateWeekDays_ForValidInputs_ReturnsCountOfWeekDays(string startDate, string endDate, int year, int expectedOutput)
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

            List<Holiday> holidays_2015 = GetHolidayList();

            var options = new DbContextOptionsBuilder<HolidayContext>()
                .UseInMemoryDatabase(databaseName: "CustomDbInMemory")
                .Options;

            // Insert seed data into the database using one instance of the context
            using (context = new HolidayContext(options))
            {
                context.PublicHolidays.Add(new PublicHolidays { Year = year, Holidays = holidays_2015 });
                context.SaveChanges();


                var publicHolidays = context.PublicHolidays.Where(x => x.Year == year).Include(h => h.Holidays).FirstOrDefault();
                var validHolidays = publicHolidays.Holidays.Where(x => DateTime.Parse(x.Date) > start && DateTime.Parse(x.Date) < end);

                var totalHolidaysCount = validHolidays.Where(x => x.Substitute == true ||
                                            (DateTime.Parse(x.Date).DayOfWeek != DayOfWeek.Saturday
                                            && DateTime.Parse(x.Date).DayOfWeek != DayOfWeek.Sunday))
                                            .Count();



                //ACT
                result = await _weekDayService.CalculateWeekdaysAsync(input) - totalHolidaysCount;
            }

            //ASSERT
            Assert.That(result == expectedOutput);
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
            return new List<Holiday> {
                new Holiday { Country = "AU", Date = "2015-01-01", End = DateTime.Parse("2015-01-02"),  IsPublic = true,
                    Name = "New Year's Day", Start = DateTime.Parse("2015-01-01"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-01-26", End = DateTime.Parse("2015-01-27"),  IsPublic = true,
                    Name = "Australia Day", Start = DateTime.Parse("2015-01-26"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-04-03", End = DateTime.Parse("2015-04-04"), IsPublic = true,
                    Name = "Good Friday", Start = DateTime.Parse("2015-04-03"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-04-04", End = DateTime.Parse("2015-04-05"), IsPublic = true,
                    Name = "Easter Saturday", Start = DateTime.Parse("2015-04-04"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-04-06", End = DateTime.Parse("2015-04-07"), IsPublic = true,
                    Name = "Easter Monday", Start = DateTime.Parse("2015-04-06"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-04-25", End = DateTime.Parse("2015-04-26"), IsPublic = true,
                    Name = "ANZAC Day", Start = DateTime.Parse("2015-04-25"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-04-27", End = DateTime.Parse("2015-04-28"), IsPublic = true,
                    Name = "ANZAC Day (substitute day)", Start = DateTime.Parse("2015-04-27"), Substitute = true, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-06-08", End = DateTime.Parse("2015-06-09"), IsPublic = true,
                    Name = "Queen's Birthday", Start = DateTime.Parse("2015-06-08"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-12-25", End = DateTime.Parse("2015-12-26"), IsPublic = true,
                    Name = "Christmas Day", Start = DateTime.Parse("2015-12-25"), Substitute = false, Type = "public" },
                new Holiday { Country = "AU", Date = "2015-12-26", End = DateTime.Parse("2015-12-27"), IsPublic = true,
                    Name = "Boxing Day", Start = DateTime.Parse("2015-12-26"), Substitute = false, Type = "public" },
                 new Holiday { Country = "AU", Date = "2015-12-28", End = DateTime.Parse("2015-12-29"), IsPublic = true,
                    Name = "Boxing Day (substitute day)", Start = DateTime.Parse("2015-12-28"), Substitute = true, Type = "public" },
            };
        }

    }
}