using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Scraper.Api.Models;

namespace Scraper.Api.Services
{
    public class ScraperService
    {
        private readonly Scraper.ScraperService.ScraperServiceClient client;

        public ScraperService(Scraper.ScraperService.ScraperServiceClient client)
        {
            this.client = client;
        }

        public async Task<List<Models.ScraperSource>> ListSources(uint skip = 0, uint take = 50)
        {
            var response = await client.ListSourcesAsync(new ListSourcesRequest
            {
                Skip = skip,
                Take = take
            });

            return response.Results.Select(_ => new Models.ScraperSource
            {
                Id = _.Id,
                Collector = MapCollector(_.Collector)
            }).ToList();
        }

        public async Task<Models.ScraperSource> GetSource(string id)
        {
            var response = await client.GetSourceAsync(new GetSourceRequest
            {
                Id = id
            });

            return new Models.ScraperSource
            {
                Id = response.Result.Id,
                Collector = MapCollector(response.Result.Collector)
            };
        }

        public async Task<string> CreateSource(Models.ScraperSource source)
        {
            var response = await client.CreateSourceAsync(new CreateSourceRequest
            {
                Collector = MapCollector(source.Collector),
            });

            return response.Id;
        }

        private Collector MapCollector(Any value)
        {
            if (value.Is(WebCollector.Descriptor) && value.TryUnpack<WebCollector>(out var collector))
            {
                return new Api.Models.WebCollector
                {
                    Target = collector.Target
                };
            }

            throw new System.Exception("Unknown collector type");
        }

        private Any MapCollector(Models.Collector collector)
        {
            if (collector is Models.WebCollector value)
            {
                return Any.Pack(new WebCollector
                {
                    Target = value.Target
                });
            }

            throw new System.Exception("Unknown collector type");
        }
    }
}