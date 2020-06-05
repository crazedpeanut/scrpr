using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Scraper;

namespace ScraperService.Services
{
    public class ResultsService : SchedulerResults.SchedulerResultsBase
    {
        private readonly ILogger<ResultsService> _logger;
        public ResultsService(ILogger<ResultsService> logger)
        {
            _logger = logger;
        }

        public override Task Query(QueryRequest request, Grpc.Core.IServerStreamWriter<QueryResponse> responseStream, Grpc.Core.ServerCallContext context)
        {
            return base.Query(request, responseStream, context);
        }
    }
}
