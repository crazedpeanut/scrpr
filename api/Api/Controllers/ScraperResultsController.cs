using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Api.Services;
using Api.Models;
using MongoDB.Driver;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperResultsController : ControllerBase
    {
        private readonly ScraperResultRespository resultRespository;

        public ScraperResultsController(ScraperResultRespository resultRespository)
        {
            this.resultRespository = resultRespository;
        }

        [HttpGet("{url}")]
        public async Task<IActionResult> List(Uri url, CancellationToken cancellationToken)
        {
            // FIXME: Dont pass url as route param, too brittle!
            var filter = Builders<ScraperResult>.Filter.Eq("Url", WebUtility.UrlDecode(url.ToString()));

            var results = await resultRespository.Get(filter, cancellationToken);

            return Ok(results);
        }
    }
}
