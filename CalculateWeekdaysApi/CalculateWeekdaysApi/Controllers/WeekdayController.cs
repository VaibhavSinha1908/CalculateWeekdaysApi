using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalculateWeekdaysApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeekdayController : ControllerBase
    {
        private readonly IWeekdayService service;

        public WeekdayController(IWeekdayService service)
        {
            this.service = service;
        }

        // POST: api/Weekday
        [HttpPost]
        public IActionResult Post([FromBody] Date input)
        {
            var result = service.CalculateWeekdays(input);
            return Ok(result);
        }


    }
}
