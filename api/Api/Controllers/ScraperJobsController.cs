using System.Text.RegularExpressions;
using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Api.Services;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperJobsController : ControllerBase
    {
        private readonly ScraperScheduler scraperScheduler;

        public ScraperJobsController(ScraperScheduler scraperScheduler)
        {
            this.scraperScheduler = scraperScheduler;
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleJob(ScraperJob job, CancellationToken cancellationToken)
        {
            await scraperScheduler.Begin(
                new ScraperJob { Url = job.Url }, cancellationToken);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetScheduledJob(CancellationToken cancellationToken)
        {
            return Ok(await scraperScheduler.List(cancellationToken));
        }
    }
}
