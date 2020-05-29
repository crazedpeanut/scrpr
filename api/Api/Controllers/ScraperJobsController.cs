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
using HtmlAgilityPack;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScraperJobsController : ControllerBase
    {
        private readonly ILogger<ScraperJobsController> logger;
        private readonly ScraperService scraperService;
        private static ScraperResult result;

        public ScraperJobsController(ILogger<ScraperJobsController> logger, IHttpClientFactory httpClient)
        {
            this.logger = logger;
            this.scraperService = new ScraperService(
                httpClient.CreateClient(),
                new List<IEntityExtractor>{
                    new PhoneNumberEntityExtractor()
                });
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

    public class ScraperResult
    {
        public ScraperResult(List<Entity> entities)
        {
            Entities = entities;
        }

        public List<Entity> Entities { get; set; }
    }

    public class ScraperService
    {
        private readonly HttpClient httpClient;
        private readonly IEnumerable<IEntityExtractor> entityExtractors;

        public ScraperService(HttpClient httpClient, IEnumerable<IEntityExtractor> entityExtractors)
        {
            this.httpClient = httpClient;
            this.entityExtractors = entityExtractors;
        }

        public async Task<ScraperResult> ScrapeUrl(Uri uri, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync(uri, cancellationToken);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return AnalyseContent(content);
        }

        private ScraperResult AnalyseContent(string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);

            var textNodes = document
                .DocumentNode
                .DescendantsAndSelf()
                .Where(node => !node.HasChildNodes && !string.IsNullOrEmpty(node.InnerText));

            var entities = textNodes
                .SelectMany(node => entityExtractors.SelectMany(extractor => extractor.Extract(node.InnerText)))
                .ToList();

            return new ScraperResult(entities);
        }
    }


    public interface IEntityExtractor
    {
        List<Entity> Extract(string content);
    }

    public class PhoneNumberEntityExtractor : IEntityExtractor
    {
        public const string Name = "PhoneNumber";
        private readonly Regex pattern = new Regex("(?<!\\d)\\d{10}(?!\\d)");
        private readonly Regex whitespacePattern = new Regex("\\s");
        public List<Entity> Extract(string content)
        {
            return pattern.Matches(whitespacePattern.Replace(content, "")).Select(m => new Entity(Name, m.Value, content)).ToList();
        }
    }

    public class Entity
    {
        public Entity(string name, string raw, string source)
        {
            Name = name;
            Raw = raw;
            Source = source;
        }
        public string Name { get; set; }
        public string Raw { get; set; }
        public string Source { get; }
    }

    public class ScraperJob
    {
        public Uri Url { get; set; }
    }
}
