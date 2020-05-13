using CalculateWeekdaysApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Repositories
{
    public interface IServiceDbRepository
    {
        Task<List<Holidays>> GetAllHolidaysListAsync();
    }
}
