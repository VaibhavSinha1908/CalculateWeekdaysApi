using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CalculateWeekdaysApi.Models.Entities
{
    public class PublicHolidays
    {
        [Key]
        public int Id { get; set; }

        public int Year { get; set; }

        public List<Holiday> Holidays { get; set; }
    }
}
