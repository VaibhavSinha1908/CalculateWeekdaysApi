using CalculateWeekdaysApi.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Repositories
{
    public interface IServiceDbRepository
    {
        Task<bool> UpdateHolidaysAsync(IEnumerable<Holiday> holidays, int year);
        Task<IEnumerable<Holiday>> GetHolidaysListAsync(int year);
    }
}
