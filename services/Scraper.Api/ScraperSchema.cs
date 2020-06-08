using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using static Scraper.Contracts.Contracts;
using Scraper.Api.Models.GraphQL;

namespace Scraper.Api
{
    public static class SchemaBuilder
    {
        public static ISchema Create(IServiceProvider serviceProvider)
        {
            return Schema.For(
                GQLContracts.Scraper,
                _ =>
                {
                    _.ServiceProvider = serviceProvider;
                    _.Types.Include<Query>();
                    _.Types.Include<WebCollectorType>();
                    _.Types.Include<CollectorType>();
                });
        }
    }

    public class Query : ObjectGraphType
    {
        private readonly Services.ScraperService scraperService;

        public Query(Services.ScraperService scraperService)
        {
            this.scraperService = scraperService;
        }

        [GraphQLMetadata("sources")]
        public Task<List<Models.ScraperSource>> GetSources(int skip, int take)
        {
            return scraperService.ListSources((uint)skip, (uint)take);
        }

        [GraphQLMetadata("source")]
        public Task<Models.ScraperSource> GetSource(string id)
        {
            return scraperService.GetSource(id);
        }
    }
}