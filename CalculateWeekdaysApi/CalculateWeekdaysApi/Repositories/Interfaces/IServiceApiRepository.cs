using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Models.ApiResponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Repositories
{
    public interface IServiceApiRepository
    {
        Task<IEnumerable<Holiday>> GetHolidaysList(int year);
    }
}
