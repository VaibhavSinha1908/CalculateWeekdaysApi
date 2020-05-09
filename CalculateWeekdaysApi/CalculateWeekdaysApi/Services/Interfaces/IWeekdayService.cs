using CalculateWeekdaysApi.Models;

namespace CalculateWeekdaysApi.Services
{
    public interface IWeekdayService
    {
        int CalculateWeekdays(Date input);
    }
}
