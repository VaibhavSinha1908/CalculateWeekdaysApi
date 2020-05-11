using CalculateWeekdaysApi.Models;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Services
{
    public interface IWeekdayService
    {
        Task<int> CalculateWeekdaysAsync(InputDates input);
    }
}
