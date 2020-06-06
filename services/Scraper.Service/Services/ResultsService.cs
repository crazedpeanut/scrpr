using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Scraper;
using Scraper.Service.Data.Services;

namespace Scraper.Service.Services
{
    public class ResultsService : SchedulerResults.SchedulerResultsBase
    {
        private readonly ScraperResultRepository resultRepository;
        public ResultsService(ScraperResultRepository resultRepository)
        {
            this.resultRepository = resultRepository;
        }

        public override async Task<QueryResponse> Query(QueryRequest request, ServerCallContext context)
        {
            var results = await resultRepository.Get(new Uri(request.Url));

            var response = new QueryResponse();
            response.Results.AddRange(results.Select(r => new QueryResponseItem
            {
                Url = r.Url.ToString()
            }));
            return response;
        }
    }
}
