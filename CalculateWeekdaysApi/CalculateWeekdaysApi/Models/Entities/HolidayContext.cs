using Microsoft.EntityFrameworkCore;

namespace CalculateWeekdaysApi.Models.Entities
{
    public class HolidayContext : DbContext
    {

        public HolidayContext(DbContextOptions<HolidayContext> options) : base(options)
        {

        }

        public DbSet<PublicHolidays> PublicHolidays { get; set; }
    }
}
