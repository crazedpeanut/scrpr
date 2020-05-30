using System.Text.RegularExpressions;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using PuppeteerSharp;
using HtmlAgilityPack;
using Api.Services;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperJobsController : ControllerBase
    {
        private readonly ILogger<ScraperJobsController> logger;
        private readonly ScraperService scraperService;
        private static ScraperResult result;

        public ScraperJobsController(ILogger<ScraperJobsController> logger)
        {
            this.logger = logger;
            this.scraperService = new ScraperService(
               new ScraperFactory(new List<IEntityExtractor>{
                    new PhoneNumberEntityExtractor()
                }));
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleJob(ScraperJob job, CancellationToken cancellationToken)
        {
            result = await scraperService.ScrapeUrl(job.Url, cancellationToken);

            logger.LogInformation(JsonConvert.SerializeObject(result));

            return Ok();
        }

        [HttpGet]
        public IActionResult GetScheduledJob()
        {
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
