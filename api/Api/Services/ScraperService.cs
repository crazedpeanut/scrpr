using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PuppeteerSharp;
using MongoDB.Bson.Serialization.Attributes;
using Api.Models;

namespace Api.Services
{
    public class ScraperService
    {
        private readonly ScraperFactory scraperFactory;

        public ScraperService(ScraperFactory scraperFactory)
        {
            this.scraperFactory = scraperFactory;
        }

        public virtual Task<ScraperResult> ScrapeUrl(Uri url, CancellationToken cancellationToken)
        {
            return scraperFactory.GetScraper(url).Scrape(cancellationToken);
        }
    }

    public abstract class Scraper
    {
        protected virtual Browser Browser { get; set; }
        public Uri Url { get; }

        private readonly IEnumerable<IEntityExtractor> entityExtractors;

        protected Scraper(Uri url, IEnumerable<IEntityExtractor> entityExtractors)
        {
            Url = url;
            this.entityExtractors = entityExtractors;
        }

        public async virtual Task<ScraperResult> Scrape(CancellationToken cancellationToken)
        {
            await Initialize();
            var page = await Navigate(Url, cancellationToken);
            return await Analyse(page);
        }

        protected async virtual Task Initialize()
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

            Browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });
        }

        protected async virtual Task<Page> Navigate(Uri uri, CancellationToken cancellationToken)
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(uri.ToString());
            return page;
        }

        protected async Task<ScraperResult> Analyse(Page page)
        {
            var document = new HtmlDocument();

            // TODO: Look into querying Puppeteer directly
            document.LoadHtml(await page.GetContentAsync());

            // FIXME: Crude text node extraction, need to determine which nodes we're interested in.
            // For example, not svgs, images, scripts, etc
            var textNodes = document
                .DocumentNode
                .DescendantsAndSelf()
                .Where(node =>
                    !node.HasChildNodes
                    && !string.IsNullOrEmpty(node.InnerText));

            var entities = textNodes
                .SelectMany(node => entityExtractors.SelectMany(extractor => extractor.Extract(node.InnerText)))
                .ToList();

            return new ScraperResult(Url, entities, DateTime.Now);
        }
    }

    public class ScraperFactory
    {
        private readonly IEnumerable<IEntityExtractor> entityExtractors;

        public ScraperFactory(IEnumerable<IEntityExtractor> entityExtractors)
        {
            this.entityExtractors = entityExtractors;
        }

        public Scraper GetScraper(Uri url)
        {
            return new BasicScraper(url, entityExtractors);
        }
    }

    public interface IEntityExtractor
    {
        List<Entity> Extract(string content);
    }

    public class BasicScraper : Scraper
    {
        public BasicScraper(Uri url, IEnumerable<IEntityExtractor> entityExtractors) : base(url, entityExtractors)
        {
        }
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
}