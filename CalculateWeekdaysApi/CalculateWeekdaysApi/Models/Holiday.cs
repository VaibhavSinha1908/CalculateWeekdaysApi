using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalculateWeekdaysApi.Models
{

    public class HolidayType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }


    public class Holiday
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Date { get; set; }
        public virtual WeekDays WeekDay { get; set; }
        public int? Month { get; set; }
        public int? WeekCount { get; set; }
        public int? Year { get; set; }
        public virtual HolidayType Type { get; set; }


        public int HolidaysListID { get; set; }

        [ForeignKey("HolidaysListID")]
        public virtual Holidays Holidays { get; set; }
    }

    public class WeekDays
    {
        [Key]
        public int Index { get; set; }
        public string Name { get; set; }
    }

    public class Holidays
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public virtual List<Holiday> HolidayList { get; set; }
    }

}