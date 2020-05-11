using CalculateWeekdaysApi.Models;
using CalculateWeekdaysApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CalculateWeekdaysApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class WeekdayController : ControllerBase
    {
        private readonly IWeekdayService service;
        private readonly ILogger<WeekdayController> logger;

        public WeekdayController(IWeekdayService service, ILogger<WeekdayController> logger)
        {
            this.service = service;
            this.logger = logger;
        }


       
        // POST: api/Weekday
        /// <summary>
        /// Calculates the working days between two dates in (dd/mm/yyyy).
        /// </summary>
        /// <remarks>
        /// sample request:
        /// Post /
        /// {
        ///     "StartDate":"13/8/2015",
        ///     "EndDate": "21/8/2016",
        /// }
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>The total working days for the given duration.</returns>
        /// <response code = "200">Total Workingdays</response>
        /// <response code = "400">Bad Response.</response>
        /// <response code = "500">Internal server error.(in case of exception)</response>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InputDates input)
        {
            try
            {
                logger.LogInformation($"In PostMethod: Starting the calculation for- {input.StartDate} & {input.EndDate}");
                //call the service layer.
                var result = await service.CalculateWeekdaysAsync(input);

                //check the result 
                if (result == -1)
                    return BadRequest(StatusMessages.BadStartDate);
                if (result == -2)
                    return BadRequest(StatusMessages.BadEndDate);
                if (result == -3)
                    return BadRequest(StatusMessages.BadStartEndDate);

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.Message, ex.StackTrace);
                return StatusCode(500, "Internal server error");
            }

        }


    }
}
